use super::compose_file::{self, ComposeFile};
use crate::cli::FilterOrAll;
use crate::ext::{EitherExt, IntoEither};
use crate::filter::{FilterIterExt, StackDescriptor};
use crate::terminal::Progress;
use crate::{cli::GlobalArgs, ext::IterExt, filter::IdentifyService, paths};
use anyhow::{anyhow, bail, Result};
use std::borrow::Cow;
use std::fs::{self, DirEntry};
use std::os::unix::process::CommandExt;
use std::path::{Path, PathBuf};
use std::process::Command;

const COMPOSE_BINARY: &str = "docker-compose";

const COMPOSE_FILE_NAMES: &[&str] = &[
	"compose.yaml",
	"compose.yml",
	"docker-compose.yaml",
	"docker-compose.yml",
];

pub struct Compose {
	app_path: PathBuf,
}

impl Compose {
	pub fn new(args: &GlobalArgs) -> Result<Self> {
		let app_path = args.app_path.as_ref().map_or_else(paths::default_apps, Clone::clone);

		if !app_path.is_dir() {
			bail!("app directory '{}' does not exist", app_path.to_string_lossy())
		}

		Ok(Self { app_path })
	}

	pub fn services(&self) -> Result<impl Iterator<Item = Service>> {
		Ok(self.compose_files()?.flat_map(into_services))
	}

	pub fn up(&self, stack: &StackDescriptor<Service>, extra_args: &[String]) -> Result<()> {
		self.run_service_command(&stack.stack, &["up", "-d"], extra_args, &stack.services, false)
	}

	pub fn down(&self, stack: &StackDescriptor<Service>, extra_args: &[String]) -> Result<()> {
		self.run_service_command(&stack.stack, &["down"], extra_args, &stack.services, false)
	}

	pub fn logs(&self, stack: &StackDescriptor<Service>, extra_args: &[String], exec: bool) -> Result<()> {
		self.run_service_command(&stack.stack, &["logs"], extra_args, &stack.services, exec)
	}

	pub fn pull(&self, stack: &StackDescriptor<Service>, extra_args: &[String]) -> Result<()> {
		self.run_service_command(&stack.stack, &["pull"], extra_args, &stack.services, false)
	}

	pub fn exec_stacks<F>(&self, target: FilterOrAll, f: F) -> Result<()>
	where
		F: Fn(&Self, &StackDescriptor<Service>, Progress) -> Result<()>,
	{
		self.services()?
			.either_with(target.filter(), |i, f| i.filter_services(f))
			.aggregate_services()
			.with_progress()
			.try_for_each(|(p, s)| f(self, &s, p).map_err(|e| anyhow!("{}: {e}", &s.stack)))
	}

	fn run_service_command(
		&self,
		stack: &str,
		args: &[&str],
		extra_args: &[String],
		services: &[impl IdentifyService],
		exec: bool,
	) -> Result<()> {
		let status = Command::new(COMPOSE_BINARY)
			.current_dir(self.get_stack_path(stack))
			.args(args)
			.args(extra_args)
			.args(services.iter().map(IdentifyService::service))
			.either_or(exec, |c| Err(c.exec()), Command::spawn)
			.unwrap()
			.and_then(|mut handle| handle.wait())?;

		if !status.success() {
			let exit_code = status.code().map_or_else(|| String::from("missing"), |c| c.to_string());
			bail!("docker-compose invocation failure, exit status {exit_code}")
		}

		Ok(())
	}

	fn get_stack_path(&self, stack_name: impl AsRef<Path>) -> PathBuf {
		let mut path = self.app_path.clone();
		path.push(stack_name);

		path
	}

	fn compose_files(&self) -> Result<impl Iterator<Item = (String, ComposeFile)>> {
		let services = fs::read_dir(&self.app_path)
			.map_err(|e| anyhow!("{e}"))?
			.filter_err_and(|e| log::warn!("unreadable path in app dir: {e}"))
			.filter_map(find_compose_file)
			.map(read_compose_file)
			.filter_err_and(|e| log::warn!("unreadable compose file: {e}"));

		Ok(services)
	}
}

fn find_compose_file(entry: DirEntry) -> Option<PathBuf> {
	let mut path = entry.path();

	// hidden folders are skipped
	if path.file_name()?.to_string_lossy().starts_with('.') {
		return None;
	}

	path.push("placeholder");

	// try all valid compose file names
	for name in COMPOSE_FILE_NAMES {
		path.set_file_name(name);

		if path.is_file() {
			return Some(path);
		}
	}

	// folders without compose files are skipped
	None
}

fn read_compose_file(path: PathBuf) -> Result<(String, ComposeFile)> {
	let stack = get_stack_name(&path)
		.ok_or_else(|| anyhow!("invalid path {path:?}"))?
		.into_owned();

	let file = fs::File::open(&path).map_err(|e| anyhow!("{e} ({path:?})"))?;
	let compose_file: ComposeFile = serde_yml::from_reader(file)?;

	Ok((stack, compose_file))
}

fn get_stack_name(compose_path: &Path) -> Option<Cow<str>> {
	Some(compose_path.parent()?.file_name()?.to_string_lossy())
}

fn into_services((stack, compose_file): (String, ComposeFile)) -> impl Iterator<Item = Service> {
	compose_file
		.services
		.into_iter()
		.map(move |(name, service)| Service::from_compose(stack.clone(), name, service))
}

#[derive(Debug, PartialEq, Eq, PartialOrd, Ord)]
pub struct Service {
	pub name: String,
	pub stack: String,
}

impl Service {
	fn from_compose(stack: String, name: String, _service: compose_file::Service) -> Self {
		Self { name, stack }
	}
}

impl IdentifyService for Service {
	fn stack(&'_ self) -> &'_ str {
		&self.stack
	}

	fn service(&'_ self) -> &'_ str {
		&self.name
	}
}

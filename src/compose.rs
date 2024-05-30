use crate::{cli::GlobalArgs, filter::IdentifyService, paths};
use anyhow::{bail, Result};
use docker_compose_types::{Compose as ComposeFile, Service as ComposeService};
use std::{fs, path::PathBuf};

const COMPOSE_FILE_NAME: &str = "docker-compose.yml";

pub struct Compose {
	app_path: PathBuf,
}

impl Compose {
	pub fn new(args: &GlobalArgs) -> Result<Compose> {
		let app_path = args.app_path.as_ref().map_or_else(paths::default_apps, Clone::clone);

		if !app_path.is_dir() {
			bail!("app directory '{}' does not exist", app_path.to_string_lossy())
		}

		Ok(Compose { app_path })
	}

	pub fn services(&self) -> Result<impl Iterator<Item = Service>> {
		Ok(self.compose_files()?.flat_map(get_services))
	}

	fn compose_files(&self) -> Result<impl Iterator<Item = (String, ComposeFile)>> {
		Ok(fs::read_dir(&self.app_path)?
			.filter_map(|e| e.map(|e| e.path()).ok())
			.map(read_compose_file)
			.filter_map(Result::ok))
	}
}

fn read_compose_file(mut path: PathBuf) -> Result<(String, ComposeFile)> {
	let stack = path
		.file_name()
		.expect("file must have a name")
		.to_string_lossy()
		.into_owned();

	path.push(COMPOSE_FILE_NAME);

	let compose_file: ComposeFile = serde_yml::from_reader(fs::File::open(path)?)?;
	Ok((stack, compose_file))
}

fn get_services((stack, compose_file): (String, ComposeFile)) -> impl Iterator<Item = Service> {
	compose_file
		.services
		.0
		.into_iter()
		.map(move |(name, service)| Service::from_compose(stack.clone(), name, service))
		.filter_map(Result::ok)
}

#[derive(Debug)]
pub struct Service {
	pub name: String,
	pub stack: String,
}

impl Service {
	fn from_compose(stack: String, name: String, _service: Option<ComposeService>) -> Result<Service> {
		Ok(Service { stack, name })
	}
}

impl IdentifyService for Service {
	fn stack(&'_ self) -> &'_ str {
		&self.stack
	}

	fn service(&'_ self) -> &'_ str {
		&self.name
	}

	fn into_service(self) -> String {
		self.name
	}
}

use crate::cli::{FilterOrAll, GlobalArgs};
use crate::docker::compose::Compose;
use crate::ext::IntoEither;
use crate::filter::{IterExt, StackDescriptor};
use anyhow::{anyhow, Result};
use std::process::ExitCode;

/// Create and start stacks
#[derive(clap::Args, Debug)]
pub struct UpArgs {
	#[command(flatten)]
	target: FilterOrAll,

	/// Recreate services even if they are up to date
	#[arg(short, long)]
	force_recreate: bool,

	/// Additional arguments passed to the docker-compose executable
	#[arg(last = true)]
	compose_args: Vec<String>,
}

pub async fn exec_up(global_args: GlobalArgs, args: UpArgs) -> Result<ExitCode> {
	let mut extra_args = args.compose_args;

	if args.force_recreate {
		extra_args.push(String::from("--force-recreate"));
	}

	exec(global_args, args.target, |c, s| c.up(s, &extra_args)).await
}

/// Stop and remove stacks
#[derive(clap::Args, Debug)]
pub struct DownArgs {
	#[command(flatten)]
	target: FilterOrAll,

	/// Remove undefined services
	#[arg(short, long)]
	remove_orphans: bool,

	/// Additional arguments passed to the docker-compose executable
	#[arg(last = true)]
	compose_args: Vec<String>,
}

pub async fn exec_down(global_args: GlobalArgs, args: DownArgs) -> Result<ExitCode> {
	let mut extra_args = args.compose_args;

	if args.remove_orphans {
		extra_args.push(String::from("--remove-orphans"));
	}

	exec(global_args, args.target, |c, s| c.down(s, &extra_args)).await
}

async fn exec<F>(global_args: GlobalArgs, target: FilterOrAll, f: F) -> Result<ExitCode>
where
	F: Fn(&Compose, &StackDescriptor) -> Result<()>,
{
	let compose = Compose::new(&global_args)?;

	let stacks = compose
		.services()?
		.either_with(target.filter(), |i, f| i.filter_services(f))
		.aggregate_services();

	for stack in stacks {
		f(&compose, &stack).map_err(|e| anyhow!("{}: {e}", &stack.stack))?;
	}

	Ok(ExitCode::SUCCESS)
}

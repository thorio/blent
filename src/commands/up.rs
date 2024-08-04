use crate::cli::{FilterOrAll, GlobalArgs};
use crate::docker::compose::Compose;
use anyhow::Result;
use std::process::ExitCode;

/// Create and start stacks
#[derive(clap::Args, Debug)]
pub struct Args {
	#[command(flatten)]
	target: FilterOrAll,

	/// Recreate services even if they are up to date
	#[arg(short, long)]
	force_recreate: bool,

	/// Additional arguments passed to the docker-compose executable
	#[arg(last = true)]
	compose_args: Vec<String>,
}

pub fn exec(global_args: GlobalArgs, args: Args) -> Result<ExitCode> {
	let mut extra_args = args.compose_args;

	if args.force_recreate {
		extra_args.push(String::from("--force-recreate"));
	}

	Compose::new(&global_args)?.exec_stacks(args.target, |c, s| c.up(s, &extra_args))?;

	Ok(ExitCode::SUCCESS)
}

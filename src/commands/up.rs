use crate::cli::FilterOrAll;
use crate::services::Services;
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

pub fn exec(services: Services, args: Args) -> Result<ExitCode> {
	let mut extra_args = args.compose_args;

	if args.force_recreate {
		extra_args.push(String::from("--force-recreate"));
	}

	services
		.compose()?
		.exec_stacks(args.target, |c, s| c.up(s, &extra_args))?;

	Ok(ExitCode::SUCCESS)
}

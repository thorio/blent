use crate::cli::FilterOrAll;
use crate::services::Services;
use anyhow::Result;
use std::process::ExitCode;

/// Stop and remove stacks
#[derive(clap::Args, Debug)]
pub struct Args {
	#[command(flatten)]
	target: FilterOrAll,

	/// Remove undefined services
	#[arg(short, long)]
	remove_orphans: bool,

	/// Additional arguments passed to the docker-compose executable
	#[arg(last = true)]
	compose_args: Vec<String>,
}

pub fn exec(services: Services, args: Args) -> Result<ExitCode> {
	let mut extra_args = args.compose_args;

	if args.remove_orphans {
		extra_args.push(String::from("--remove-orphans"));
	}

	services
		.compose()?
		.exec_stacks(args.target, |c, s| c.down(s, &extra_args))?;

	Ok(ExitCode::SUCCESS)
}

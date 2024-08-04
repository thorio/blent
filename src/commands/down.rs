use crate::cli::{FilterOrAll, GlobalArgs};
use crate::docker::compose::Compose;
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

pub fn exec(global_args: GlobalArgs, args: Args) -> Result<ExitCode> {
	let mut extra_args = args.compose_args;

	if args.remove_orphans {
		extra_args.push(String::from("--remove-orphans"));
	}

	Compose::new(&global_args)?.exec_stacks(args.target, |c, s| c.down(s, &extra_args))?;

	Ok(ExitCode::SUCCESS)
}

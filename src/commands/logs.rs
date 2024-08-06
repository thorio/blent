use crate::filter::{FilterIterExt, ServiceFilter};
use crate::services::Services;
use anyhow::{bail, Result};
use itertools::Itertools;
use std::process::ExitCode;

/// View output from services
#[derive(clap::Args, Debug)]
pub struct Args {
	/// Operate only on matching services, Format: "stack:service"
	#[arg(required = true)]
	filter: Vec<ServiceFilter>,

	/// Follow log output
	#[arg(short, long)]
	follow: bool,

	/// Additional arguments passed to the docker-compose executable
	#[arg(last = true)]
	compose_args: Vec<String>,
}

pub fn exec(services: Services, args: Args) -> Result<ExitCode> {
	let compose = services.compose()?;

	let stacks = compose
		.services()?
		.filter_services(&args.filter)
		.aggregate_services()
		.collect_vec();

	if stacks.len() > 1 {
		bail!("cannot operate on multiple stacks")
	}

	let Some(stack) = stacks.first() else {
		bail!("no matching services found")
	};

	let mut extra_args = args.compose_args;

	if args.follow {
		extra_args.push(String::from("--follow"));
	}

	compose.logs(stack, &extra_args, true)?;

	Ok(ExitCode::SUCCESS)
}

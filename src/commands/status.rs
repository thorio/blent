use crate::cli::GlobalArgs;
use crate::docker;
use crate::filter::IteratorExt;
use crate::filter::ServiceFilter;
use anyhow::Result;
use std::process::ExitCode;

/// Print a summary of running stacks
#[derive(clap::Args, Debug)]
pub struct Args {
	filter: Vec<ServiceFilter>,
}

pub async fn exec(global_args: GlobalArgs, args: Args) -> Result<ExitCode> {
	let docker = docker::connect(&global_args)?;

	let services = docker.services().await?.filter_services(&args.filter);

	for service in services {
		println!("{}: {} ({})", service.stack, service.name, service.status)
	}

	Ok(ExitCode::SUCCESS)
}

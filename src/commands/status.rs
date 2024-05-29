use crate::cli::GlobalArgs;
use crate::docker;
use crate::filter::{FilterIterExt, ServiceFilter};
use crate::iterext::IteratorExt;
use anyhow::Result;
use std::process::ExitCode;

/// Print a summary of stacks
#[derive(clap::Args, Debug)]
pub struct Args {
	filter: Vec<ServiceFilter>,
}

pub async fn exec(global_args: GlobalArgs, args: Args) -> Result<ExitCode> {
	let docker = docker::connect(&global_args)?;

	let services = docker
		.services()
		.await?
		.when(!args.filter.is_empty(), |i| i.filter_services(&args.filter));

	for service in services {
		println!("{}: {} ({})", service.stack, service.name, service.status)
	}

	Ok(ExitCode::SUCCESS)
}

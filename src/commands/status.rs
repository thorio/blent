use crate::cli::GlobalArgs;
use crate::docker;
use crate::filter::IteratorExt;
use crate::filter::ServiceFilter;
use anyhow::Result;
use itertools::Itertools;
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
		.filter_services(&args.filter)
		.aggregate_services()
		.sorted()
		.collect_vec();

	dbg!(services);

	// for service in services {
	// 	println!("{}: {} ({})", service.stack, service.name, service.status)
	// }

	Ok(ExitCode::SUCCESS)
}

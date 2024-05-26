use crate::{cli::GlobalArgs, docker};
use anyhow::Result;
use std::process::ExitCode;

/// Print a summary of running stacks
#[derive(clap::Args, Debug)]
pub struct Args {}

pub async fn exec(global_args: GlobalArgs, _args: Args) -> Result<ExitCode> {
	let docker = docker::connect(&global_args)?;

	let services = docker.services().await?;

	for service in services {
		println!("{}: {} ({})", service.stack, service.name, service.status)
	}

	Ok(ExitCode::SUCCESS)
}

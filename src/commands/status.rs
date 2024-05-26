use crate::{cli::GlobalArgs, docker};
use anyhow::Result;
use bollard::container::ListContainersOptions;
use std::process::ExitCode;

/// Print a summary of running stacks
#[derive(clap::Args, Debug)]
pub struct Args {}

pub async fn exec(global_args: GlobalArgs, _args: Args) -> Result<ExitCode> {
	let docker = docker::connect(&global_args)?;

	let list_options = ListContainersOptions::<String> {
		all: true,
		..Default::default()
	};

	let containers = docker.list_containers(Some(list_options)).await?;

	for container in containers {
		println!("{:?}", container.names)
	}

	Ok(ExitCode::SUCCESS)
}

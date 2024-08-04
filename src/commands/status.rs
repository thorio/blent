use crate::cli::GlobalArgs;
use crate::docker::compose::Compose;
use crate::docker::daemon::Docker;
use crate::ext::IntoEither;
use crate::filter::{FilterIterExt, ServiceFilter};
use anyhow::Result;
use itertools::Itertools;
use std::collections::HashMap;
use std::process::ExitCode;

/// Print a summary of stacks
#[derive(clap::Args, Debug)]
pub struct Args {
	filter: Vec<ServiceFilter>,

	// Discard values after --
	#[arg(last = true, hide = true)]
	_discard: Vec<String>,
}

pub async fn exec(global_args: GlobalArgs, args: Args) -> Result<ExitCode> {
	let docker = Docker::connect(&global_args)?;
	let compose = Compose::new(&global_args)?;

	let docker_services = docker.services().await?.collect_vec();
	let docker_service_map = docker_services
		.iter()
		.map(|s| ((&s.stack, &s.name), s))
		.collect::<HashMap<_, _>>();

	let services = compose
		.services()?
		.either(!args.filter.is_empty(), |i| i.filter_services(&args.filter));

	for service in services {
		let docker_service = docker_service_map
			.get(&(&service.stack, &service.name))
			.map_or("Missing", |s| s.status.as_str());

		println!("{}: {}, ({})", service.stack, service.name, docker_service);
	}

	Ok(ExitCode::SUCCESS)
}

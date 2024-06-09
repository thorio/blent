use crate::cli::GlobalArgs;
use crate::docker::compose::Compose;
use crate::ext::IntoEither;
use crate::filter::{IterExt, ServiceFilter};
use anyhow::{anyhow, Result};
use std::process::ExitCode;

/// Create and start stacks
#[derive(clap::Args, Debug)]
pub struct Args {
	/// Operate only on matching services. Format: "stack:service"
	#[arg(required = true)]
	filter: Option<Vec<ServiceFilter>>,

	/// Operate on all known services
	#[arg(short, long, conflicts_with = "filter")]
	all: bool,

	/// Additional arguments passed to the docker-compose executable
	#[arg(last = true)]
	compose_args: Vec<String>,
}

pub async fn exec(global_args: GlobalArgs, args: Args) -> Result<ExitCode> {
	let compose = Compose::new(&global_args)?;

	let stacks = compose
		.services()?
		.either(!args.all, |i| i.filter_services(args.filter.as_ref().unwrap()))
		.aggregate_services();

	for stack in stacks {
		compose.down(&stack).map_err(|e| anyhow!("{}: {e}", &stack.stack))?;
	}

	Ok(ExitCode::SUCCESS)
}

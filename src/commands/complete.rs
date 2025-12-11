use crate::{filter::FilterIterExt, services::Services};
use anyhow::Result;
use clap::ValueEnum;
use itertools::Itertools;
use std::process::ExitCode;

/// Used for shell completions; Not stable!
#[derive(clap::Args, Debug)]
pub struct Args {
	kind: CompleteKind,

	#[arg(required = true)]
	line: String,
}

#[derive(Copy, Clone, Debug, ValueEnum)]
pub enum CompleteKind {
	Filter,
}

pub fn exec(services: Services, args: Args) -> Result<ExitCode> {
	let stacks = services.compose()?.services()?.sorted().aggregate_services();

	for stack in stacks {
		println!("{}", stack.stack);
		for service in stack.services {
			println!("{}:{}", service.stack, service.name);
		}
	}

	Ok(ExitCode::SUCCESS)
}

use crate::{commands, filter::ServiceFilter};
use clap::{Parser, Subcommand};
use clap_verbosity_flag::{Verbosity, WarnLevel};
use std::path::PathBuf;

pub fn parse() -> Args {
	Args::parse()
}

#[derive(Parser, Debug)]
#[command(author, version, about)]
#[command(
	disable_help_flag = true,
	disable_help_subcommand = true,
	disable_version_flag = true
)]
pub struct Args {
	#[command(flatten)]
	pub global: GlobalArgs,

	#[command(subcommand)]
	pub command: Command,

	#[command(flatten)]
	pub verbosity: Verbosity<WarnLevel>,

	/// Print help
	#[clap(long, global = true, action = clap::ArgAction::HelpLong)]
	_help: Option<bool>,

	/// Print version
	#[clap(long, action = clap::ArgAction::Version)]
	_version: Option<bool>,
}

#[derive(clap::Args, Debug)]
pub struct GlobalArgs {
	/// Directory in which to look for stacks [default: ~/apps]
	#[arg(long, value_name = "PATH", global = true)]
	pub app_path: Option<PathBuf>,
}

#[derive(Subcommand, Debug)]
pub enum Command {
	Status(commands::status::Args),
	Up(commands::up::Args),
	Down(commands::down::Args),
	Logs(commands::logs::Args),
}

#[derive(clap::Args, Debug)]
pub struct FilterOrAll {
	/// Operate only on matching services. Format: "stack:service"
	#[arg(required = true)]
	filter: Option<Vec<ServiceFilter>>,

	/// Operate on all known services
	#[arg(short, long, conflicts_with = "filter")]
	all: bool,
}

impl FilterOrAll {
	pub fn filter(&self) -> Option<&Vec<ServiceFilter>> {
		// Due to `filter` and `all` being mutually exclusive and required,
		// `filter` will always be None when `all` is true, and vice versa.
		self.filter.as_ref()
	}
}

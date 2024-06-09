use crate::commands;
use clap::{Args, Parser, Subcommand};
use clap_verbosity_flag::{Verbosity, WarnLevel};
use std::path::PathBuf;

pub fn parse() -> CliArgs {
	CliArgs::parse()
}

#[derive(Parser, Debug)]
#[command(author, version, about)]
#[command(
	disable_help_flag = true,
	disable_help_subcommand = true,
	disable_version_flag = true
)]
pub struct CliArgs {
	#[command(flatten)]
	pub global: GlobalArgs,

	#[command(subcommand)]
	pub command: Command,

	/// enable debug loglevel
	#[command(flatten)]
	pub verbosity: Verbosity<WarnLevel>,

	/// Print help
	#[clap(long, global = true, action = clap::ArgAction::HelpLong)]
	_help: Option<bool>,

	/// Print version
	#[clap(long, action = clap::ArgAction::Version)]
	_version: Option<bool>,
}

#[derive(Args, Debug)]
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
}

use crate::commands;
use clap::{Args, Parser, Subcommand};
use clap_verbosity_flag::{Verbosity, WarnLevel};

pub fn parse() -> CliArgs {
	CliArgs::parse()
}

#[derive(Parser, Debug)]
#[command(author, version, about, long_about = None)]
#[command(propagate_version = true)]
pub struct CliArgs {
	#[command(flatten)]
	pub global: GlobalArgs,

	#[command(subcommand)]
	pub command: Command,

	/// enable debug loglevel
	#[command(flatten)]
	pub verbosity: Verbosity<WarnLevel>,
}

#[derive(Args, Debug)]
pub struct GlobalArgs {}

#[derive(Subcommand, Debug)]
pub enum Command {
	Status(commands::status::Args),
}

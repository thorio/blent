use clap::Parser;
use clap_verbosity_flag::{Verbosity, WarnLevel};

pub fn parse() -> Args {
	Args::parse()
}

#[derive(Parser, Debug)]
#[command(author, version, about, long_about = None)]
#[command(propagate_version = true)]
pub struct Args {
	/// enable debug loglevel
	#[command(flatten)]
	pub verbosity: Verbosity<WarnLevel>,
}

use anyhow::Result;
use cli::CliArgs;
use log::LevelFilter;
use std::process::ExitCode;

mod cli;
mod commands;
mod docker;
mod filter;
mod iterext;

#[tokio::main(flavor = "current_thread")]
async fn main() -> ExitCode {
	color_eyre::install().unwrap();

	let args = cli::parse();

	init_log(args.verbosity.log_level_filter());

	match run_command(args).await {
		Ok(code) => code,
		Err(err) => {
			log::error!("{}", err);
			ExitCode::FAILURE
		}
	}
}

async fn run_command(args: CliArgs) -> Result<ExitCode> {
	use cli::Command as cmd;

	match args.command {
		cmd::Status(a) => commands::status::exec(args.global, a).await,
	}
}

fn init_log(_level: LevelFilter) {
	// TODO

	log::trace!("hello world");
}

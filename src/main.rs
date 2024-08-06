use anyhow::Result;
use log::LevelFilter;
use services::Services;
use std::process::ExitCode;

mod cli;
mod commands;
mod ext;
mod filter;
mod paths;
mod services;

#[tokio::main(flavor = "current_thread")]
#[allow(clippy::unwrap_used)]
async fn main() -> ExitCode {
	color_eyre::install().unwrap();

	let args = cli::parse();

	init_log(args.verbosity.log_level_filter()).unwrap();

	match run_command(args).await {
		Ok(code) => code,
		Err(err) => {
			log::error!("{err}");
			ExitCode::FAILURE
		}
	}
}

async fn run_command(args: cli::Args) -> Result<ExitCode> {
	use cli::Command as cmd;

	let services = Services::new(args.global);

	match args.command {
		cmd::Status(a) => commands::status::exec(services, a).await,
		cmd::Up(a) => commands::up::exec(services, a),
		cmd::Down(a) => commands::down::exec(services, a),
		cmd::Logs(a) => commands::logs::exec(services, a),
	}
}

fn init_log(level: LevelFilter) -> Result<()> {
	stderrlog::new().verbosity(level).init()?;

	log::trace!("hello world");

	Ok(())
}

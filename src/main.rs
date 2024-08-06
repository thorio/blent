use anyhow::Result;
use log::LevelFilter;
use std::process::ExitCode;

mod cli;
mod commands;
mod docker;
mod ext;
mod filter;
mod paths;

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

	match args.command {
		cmd::Status(a) => commands::status::exec(args.global, a).await,
		cmd::Up(a) => commands::up::exec(args.global, a),
		cmd::Down(a) => commands::down::exec(args.global, a),
		cmd::Logs(a) => commands::logs::exec(args.global, a),
	}
}

fn init_log(level: LevelFilter) -> Result<()> {
	stderrlog::new().verbosity(level).init()?;

	log::trace!("hello world");

	Ok(())
}

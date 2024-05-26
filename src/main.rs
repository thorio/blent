mod cli;

fn main() {
	color_eyre::install().unwrap();

	let args = cli::parse();

	init_log(args.verbosity.log_level());
	run();

	log::trace!("exiting");
}

fn init_log(level: impl Into<stderrlog::LogLevelNum>) {
	stderrlog::new()
		.verbosity(level)
		.init()
		.expect("this must never be called twice");

	log::trace!("hello world");
}

fn run() {
	log::info!("hello world");
}

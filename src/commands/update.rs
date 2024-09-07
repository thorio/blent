use crate::cli::FilterOrAll;
use crate::services::Services;
use crate::terminal::exec_stacks_pretty;
use anyhow::Result;
use bytesize::ByteSize;
use color_eyre::owo_colors::OwoColorize;
use std::process::ExitCode;

/// Create and start stacks
#[derive(clap::Args, Debug)]
pub struct Args {
	#[command(flatten)]
	target: FilterOrAll,

	/// Prune dangling images after updating
	#[arg(short, long)]
	prune_dangling: bool,

	/// Additional arguments passed to the docker-compose executable
	#[arg(last = true)]
	compose_args: Vec<String>,
}

pub async fn exec(services: Services, args: Args) -> Result<ExitCode> {
	exec_stacks_pretty(&services, args.target, |c, s| {
		c.pull(s, &args.compose_args)?;
		c.up(s, &args.compose_args)
	})?;

	if args.prune_dangling {
		prune(&services).await?;
	}

	Ok(ExitCode::SUCCESS)
}

pub async fn prune(services: &Services) -> Result<()> {
	println!("{}...", "pruning images".blue());
	let response = services.docker()?.prune_images().await?;

	println!("\npruned {} images", response.pruned_images.blue(),);
	println!("reclaimed {}", ByteSize(response.reclaimed_bytes).blue());

	Ok(())
}

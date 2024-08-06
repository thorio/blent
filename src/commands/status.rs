use crate::ext::{IntoEither, IterExt};
use crate::filter::{FilterIterExt, IdentifyService, ServiceFilter, StackDescriptor};
use crate::services::compose::{self};
use crate::services::docker::{self};
use crate::services::Services;
use anyhow::Result;
use chrono::{DateTime, Local, Utc};
use chrono_humanize::HumanTime;
use comfy_table::{presets, Cell, Color, ContentArrangement};
use comfy_table::{Row, Table};
use itertools::Itertools;
use std::process::ExitCode;

/// Print a summary of stacks
#[derive(clap::Args, Debug)]
pub struct Args {
	filter: Vec<ServiceFilter>,

	/// Show details about services
	#[arg(short, long)]
	detailed: bool,

	// Discard values after --
	#[arg(last = true, hide = true)]
	_discard: Vec<String>,
}

#[derive(Debug, PartialEq, Eq, PartialOrd, Ord)]
pub struct ServiceStatus {
	pub compose: compose::Service,
	pub daemon: Option<docker::Service>,
}

impl IdentifyService for ServiceStatus {
	fn stack(&'_ self) -> &'_ str {
		&self.compose.stack
	}

	fn service(&'_ self) -> &'_ str {
		&self.compose.name
	}
}

pub async fn exec(services: Services, args: Args) -> Result<ExitCode> {
	let stacks = get_stacks(&services, &args.filter).await?;

	let mut table = Table::new();
	table
		.load_preset(presets::NOTHING)
		.set_content_arrangement(ContentArrangement::Dynamic);

	if args.detailed {
		table.set_header(vec!["NAME", "IMAGE", "CREATED", "STATUS"]);
	}

	for stack in stacks {
		table.add_row(stack_header(&stack));

		if !args.detailed || up_count(&stack.services) == 0 {
			continue;
		}

		for service in stack.services {
			table.add_row(service_row(service));
		}
	}

	for column in table.column_iter_mut() {
		column.set_padding((0, 3));
	}

	println!("{table}");

	Ok(ExitCode::SUCCESS)
}

fn stack_header(stack: &StackDescriptor<ServiceStatus>) -> Row {
	let total = stack.services.len();
	let up = up_count(&stack.services);
	let running = running_count(&stack.services);

	let color = match up {
		0 => Color::Red,
		_ if total > running => Color::Yellow,
		_ => Color::Green,
	};

	Row::from(vec![Cell::new(format!("{} ({up}/{total})", stack.stack)).fg(color)])
}

fn service_row(service: ServiceStatus) -> Row {
	let daemon = service.daemon.unwrap_or_default();
	let date = daemon.created.and_then(format_time).unwrap_or_default();

	let color = match daemon.running {
		true => Color::Reset,
		false => Color::Red,
	};

	Row::from(vec![
		Cell::new(format!("  {}", service.compose.name)).fg(color),
		Cell::new(daemon.image),
		Cell::new(date),
		Cell::new(daemon.status).fg(color),
	])
}

fn format_time(ts: i64) -> Option<String> {
	let local = DateTime::<Utc>::from_timestamp(ts, 0)?.with_timezone(&Local);
	Some(HumanTime::from(local).to_string())
}

fn up_count(services: &[ServiceStatus]) -> usize {
	services.iter().filter_count(|s| s.daemon.is_some())
}

fn running_count(services: &[ServiceStatus]) -> usize {
	services
		.iter()
		.filter_count(|s| s.daemon.as_ref().is_some_and(|s| s.running))
}

pub async fn get_stacks(
	services: &Services,
	filter: &[ServiceFilter],
) -> Result<impl Iterator<Item = StackDescriptor<ServiceStatus>>> {
	let docker_services = services.docker()?.services().await?;

	let stacks = services
		.compose()?
		.services()?
		.sorted()
		.either(!filter.is_empty(), |i| i.filter_services(filter))
		.match_services(docker_services, |compose, running| ServiceStatus {
			compose,
			daemon: running,
		})
		.aggregate_services()
		.sorted();

	Ok(stacks)
}

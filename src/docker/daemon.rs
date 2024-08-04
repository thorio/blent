use crate::{cli::GlobalArgs, ext::IterExt, filter::IdentifyService};
use anyhow::anyhow;
use bollard::{container::ListContainersOptions, errors::Error as BollardError, secret::ContainerSummary};
use itertools::Itertools;

const COMPOSE_PROJECT_LABEL: &str = "com.docker.compose.project";
const COMPOSE_SERVICE_LABEL: &str = "com.docker.compose.service";
const DOCKER_STATE_RUNNING: &str = "running";

pub struct Docker {
	bollard: bollard::Docker,
}

impl Docker {
	pub fn connect(_args: &GlobalArgs) -> Result<Docker, BollardError> {
		let bollard = bollard::Docker::connect_with_defaults()?;

		Ok(Docker { bollard })
	}

	pub async fn services(&self) -> Result<impl Iterator<Item = Service>, BollardError> {
		let options = ListContainersOptions::<String> {
			all: true,
			..Default::default()
		};

		let services = self
			.bollard
			.list_containers(Some(options))
			.await?
			.into_iter()
			.map(TryInto::try_into)
			.filter_err_and(|e| log::debug!("skipping invalid container: {e}"))
			.sorted()
			.dedup();

		Ok(services)
	}
}

#[derive(Debug)]
pub struct Service {
	pub name: String,
	pub stack: String,
	pub status: String,
	#[allow(dead_code)]
	pub running: bool,
}

impl TryFrom<ContainerSummary> for Service {
	type Error = anyhow::Error;

	fn try_from(value: ContainerSummary) -> Result<Self, Self::Error> {
		let mut labels = value.labels.ok_or(anyhow!("no labels"))?;

		let name = labels
			.remove(COMPOSE_SERVICE_LABEL)
			.ok_or(anyhow!("no service label"))?;

		let stack = labels
			.remove(COMPOSE_PROJECT_LABEL)
			.ok_or(anyhow!("no project label"))?;

		let status = value.status.ok_or(anyhow!("no status"))?;

		let running = value.state.ok_or(anyhow!("no state"))? == DOCKER_STATE_RUNNING;

		Ok(Service {
			name,
			stack,
			status,
			running,
		})
	}
}

impl IdentifyService for Service {
	fn stack(&'_ self) -> &'_ str {
		&self.stack
	}

	fn service(&'_ self) -> &'_ str {
		&self.name
	}

	fn into_service(self) -> String {
		self.name
	}
}

impl PartialEq for Service {
	fn eq(&self, other: &Self) -> bool {
		self.name == other.name && self.stack == other.stack
	}
}

impl Eq for Service {}

impl PartialOrd for Service {
	fn partial_cmp(&self, other: &Self) -> Option<std::cmp::Ordering> {
		Some(self.cmp(other))
	}
}

impl Ord for Service {
	fn cmp(&self, other: &Self) -> std::cmp::Ordering {
		self.stack.cmp(&other.stack).then(self.name.cmp(&other.name))
	}
}

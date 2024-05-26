use crate::cli::GlobalArgs;
use bollard::{container::ListContainersOptions, errors::Error as BollardError, secret::ContainerSummary};
use itertools::Itertools;

const COMPOSE_PROJECT_LABEL: &str = "com.docker.compose.project";
const COMPOSE_SERVICE_LABEL: &str = "com.docker.compose.service";
const DOCKER_STATE_RUNNING: &str = "running";

pub fn connect(_args: &GlobalArgs) -> Result<Docker, BollardError> {
	let bollard = bollard::Docker::connect_with_defaults()?;

	Ok(Docker { bollard })
}

pub struct Docker {
	bollard: bollard::Docker,
}

impl Docker {
	pub async fn services(&self) -> Result<Vec<Service>, BollardError> {
		let options = ListContainersOptions::<String> {
			all: true,
			..Default::default()
		};

		let services = self
			.bollard
			.list_containers(Some(options))
			.await?
			.into_iter()
			.filter_map(get_service)
			.sorted()
			.dedup()
			.collect_vec();

		Ok(services)
	}
}

fn get_service(container: ContainerSummary) -> Option<Service> {
	dbg!(&container);
	let mut labels = container.labels?;

	Some(Service {
		name: labels.remove(COMPOSE_SERVICE_LABEL)?,
		stack: labels.remove(COMPOSE_PROJECT_LABEL)?,
		status: container.status?,
		running: container.state? == DOCKER_STATE_RUNNING,
	})
}

#[derive(Debug)]
pub struct Service {
	pub name: String,
	pub stack: String,
	pub status: String,
	pub running: bool,
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

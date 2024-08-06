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
	pub fn connect(_args: &GlobalArgs) -> Result<Self, BollardError> {
		let bollard = bollard::Docker::connect_with_defaults()?;

		Ok(Self { bollard })
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

#[derive(Debug, Clone)]
pub struct Service {
	pub name: String,
	pub stack: String,
	pub status: String,
	pub image: String,
	pub created: Option<i64>,
	pub running: bool,
}

impl Default for Service {
	fn default() -> Self {
		Self {
			name: Default::default(),
			stack: Default::default(),
			status: String::from("Missing"),
			image: Default::default(),
			created: Default::default(),
			running: Default::default(),
		}
	}
}

impl TryFrom<ContainerSummary> for Service {
	type Error = anyhow::Error;

	fn try_from(value: ContainerSummary) -> Result<Self, Self::Error> {
		let mut labels = value.labels.ok_or_else(|| anyhow!("no labels"))?;

		let name = labels
			.remove(COMPOSE_SERVICE_LABEL)
			.ok_or_else(|| anyhow!("no service label"))?;

		let stack = labels
			.remove(COMPOSE_PROJECT_LABEL)
			.ok_or_else(|| anyhow!("no project label"))?;

		let status = value.status.ok_or_else(|| anyhow!("no status"))?;
		let running = value.state.ok_or_else(|| anyhow!("no state"))? == DOCKER_STATE_RUNNING;
		let image = value.image.ok_or_else(|| anyhow!("no image"))?;
		let created = value.created.ok_or_else(|| anyhow!("no created date"))?;

		Ok(Self {
			name,
			stack,
			status,
			image,
			created: Some(created),
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

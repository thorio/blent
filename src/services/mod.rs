use crate::cli::GlobalArgs;
use anyhow::Result;
use compose::Compose;
use docker::Docker;
use std::cell::OnceCell;

pub mod compose;
pub mod compose_file;
pub mod docker;

pub struct Services {
	args: GlobalArgs,
	docker: OnceCell<Docker>,
	compose: OnceCell<Compose>,
}

impl Services {
	pub fn new(args: GlobalArgs) -> Self {
		Self {
			args,
			docker: OnceCell::default(),
			compose: OnceCell::default(),
		}
	}

	pub fn docker(&self) -> Result<&Docker> {
		if let Some(docker) = self.docker.get() {
			return Ok(docker);
		}

		let docker = Docker::connect(&self.args)?;
		Ok(self.docker.get_or_init(|| docker))
	}

	pub fn compose(&self) -> Result<&Compose> {
		if let Some(compose) = self.compose.get() {
			return Ok(compose);
		}

		let compose = Compose::new(&self.args)?;
		Ok(self.compose.get_or_init(|| compose))
	}
}

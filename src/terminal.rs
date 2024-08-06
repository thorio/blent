use crate::cli::FilterOrAll;
use crate::filter::StackDescriptor;
use crate::services::compose::{self, Compose};
use crate::services::Services;
use anyhow::Result;
use owo_colors::OwoColorize;
use std::fmt::{self, Display, Formatter};

pub struct Progress {
	pub current: u8,
	pub total: u8,
}

impl Progress {
	pub fn new(current: impl Into<u8>, total: u8) -> Self {
		Self {
			current: current.into(),
			total,
		}
	}
}

impl Display for Progress {
	fn fmt(&self, f: &mut Formatter<'_>) -> fmt::Result {
		write!(f, "[{}/{}]", self.current, self.total)
	}
}

pub fn exec_stacks_pretty<F>(services: &Services, target: FilterOrAll, f: F) -> Result<()>
where
	F: Fn(&Compose, &StackDescriptor<compose::Service>) -> Result<()>,
{
	services.compose()?.exec_stacks(target, |compose, stack, progress| {
		println!("{progress} {}...", stack.stack.blue());

		f(compose, stack)?;

		// TODO: clear output of inner command to present clean list
		println!("{progress} {}", stack.stack.green());

		Ok(())
	})
}

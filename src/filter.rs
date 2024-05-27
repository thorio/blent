use anyhow::{anyhow, bail};
use std::str::FromStr;

pub trait IteratorExt: Iterator {
	fn filter_services(self, filters: &[impl FilterService]) -> impl Iterator<Item = Self::Item>
	where
		Self: Sized,
		Self::Item: ServiceDesignator,
	{
		self.filter(|s| filters.iter().any(|f| f.filter(s)))
	}
}

impl<T: Iterator> IteratorExt for T {}

pub trait FilterService {
	fn filter(&self, service: &impl ServiceDesignator) -> bool;
}

pub trait ServiceDesignator {
	fn stack(&'_ self) -> &'_ str;
	fn service(&'_ self) -> &'_ str;
}

#[derive(Debug, Clone)]
pub struct ServiceFilter {
	stack: String,
	service: Option<String>,
}

impl FilterService for ServiceFilter {
	fn filter(&self, service: &impl ServiceDesignator) -> bool {
		self.stack == service.stack()
			&& (self.service.is_none() || self.service.as_ref().is_some_and(|s| s == service.service()))
	}
}

impl FromStr for ServiceFilter {
	type Err = anyhow::Error;

	fn from_str(s: &str) -> Result<Self, Self::Err> {
		let mut split = s.split(':');
		let stack = split.next().expect("split always returns at least one item").trim();
		let service = split.next().map(|s| s.trim());

		if stack.is_empty() {
			bail!("no stack specified");
		}

		if service.is_some_and(|s| s.is_empty()) {
			bail!("trailing : is not allowed")
		}

		if let Some(third) = split.next() {
			bail!("unexpected '{third}'");
		}

		Ok(Self {
			stack: stack.to_string(),
			service: service.map(str::to_string),
		})
	}
}

#[derive(Debug, Clone)]
pub struct ServiceSpecifier {
	stack: String,
	service: String,
}

impl FilterService for ServiceSpecifier {
	fn filter(&self, service: &impl ServiceDesignator) -> bool {
		self.stack == service.stack() && self.service == service.service()
	}
}

impl ServiceDesignator for ServiceSpecifier {
	fn stack(&'_ self) -> &'_ str {
		&self.stack
	}

	fn service(&'_ self) -> &'_ str {
		&self.service
	}
}

impl FromStr for ServiceSpecifier {
	type Err = anyhow::Error;

	fn from_str(s: &str) -> Result<Self, Self::Err> {
		let filter = ServiceFilter::from_str(s)?;

		Ok(Self {
			stack: filter.stack,
			service: filter.service.ok_or(anyhow!("no service specified"))?,
		})
	}
}

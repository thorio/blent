use anyhow::{anyhow, bail};
use itertools::Itertools;
use std::str::FromStr;

const ARG_SEPARATOR: char = ':';

pub trait IterExt: Iterator {
	fn filter_services(self, filters: &[impl FilterService]) -> impl Iterator<Item = Self::Item>
	where
		Self: Sized,
		Self::Item: IdentifyService,
	{
		self.filter(|s| filters.iter().any(|f| f.filter(s)))
	}

	fn aggregate_services(self) -> impl Iterator<Item = StackDescriptor>
	where
		Self: Sized,
		Self::Item: IdentifyService + Ord,
	{
		self.into_group_map_by(|s| s.stack().to_owned())
			.into_iter()
			.map(|(k, v)| StackDescriptor {
				stack: k,
				services: v.into_iter().map(IdentifyService::into_service).collect_vec(),
			})
	}
}

impl<T: Iterator> IterExt for T {}

pub trait FilterService {
	fn filter(&self, service: &impl IdentifyService) -> bool;
}

pub trait IdentifyService {
	fn stack(&'_ self) -> &'_ str;
	fn service(&'_ self) -> &'_ str;
	fn into_service(self) -> String;
}

#[derive(Debug, Clone)]
pub struct ServiceFilter {
	pub stack: String,
	pub service: Option<String>,
}

impl FilterService for ServiceFilter {
	fn filter(&self, service: &impl IdentifyService) -> bool {
		self.stack == service.stack()
			&& (self.service.is_none() || self.service.as_ref().is_some_and(|s| s == service.service()))
	}
}

impl FromStr for ServiceFilter {
	type Err = anyhow::Error;

	fn from_str(s: &str) -> Result<Self, Self::Err> {
		let mut split = s.split(ARG_SEPARATOR);
		let stack = split.next().expect("split always returns at least one item").trim();
		let service = split.next().map(str::trim);

		if stack.is_empty() {
			bail!("no stack specified");
		}

		if service.is_some_and(str::is_empty) {
			bail!("trailing '{ARG_SEPARATOR}' is not allowed")
		}

		if let Some(third) = split.next() {
			bail!("unexpected '{third}'");
		}

		Ok(Self {
			stack: stack.to_owned(),
			service: service.map(str::to_owned),
		})
	}
}

#[derive(Debug, Clone)]
pub struct ServiceDescriptor {
	pub stack: String,
	pub service: String,
}

impl FilterService for ServiceDescriptor {
	fn filter(&self, service: &impl IdentifyService) -> bool {
		self.stack == service.stack() && self.service == service.service()
	}
}

impl IdentifyService for ServiceDescriptor {
	fn stack(&'_ self) -> &'_ str {
		&self.stack
	}

	fn service(&'_ self) -> &'_ str {
		&self.service
	}

	fn into_service(self) -> String {
		self.service
	}
}

impl FromStr for ServiceDescriptor {
	type Err = anyhow::Error;

	fn from_str(s: &str) -> Result<Self, Self::Err> {
		let filter = ServiceFilter::from_str(s)?;

		Ok(Self {
			stack: filter.stack,
			service: filter.service.ok_or(anyhow!("no service specified"))?,
		})
	}
}

#[derive(Debug, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub struct StackDescriptor {
	pub stack: String,
	pub services: Vec<String>,
}

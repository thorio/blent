use anyhow::{anyhow, bail};
use itertools::Itertools;
use std::str::FromStr;

const ARG_SEPARATOR: char = ':';

pub trait FilterIterExt: Iterator + Sized {
	fn filter_services(self, filters: &[impl FilterService]) -> impl Iterator<Item = Self::Item>
	where
		Self::Item: IdentifyService,
	{
		self.filter(|s| filters.iter().any(|f| f.filter(s)))
	}

	fn aggregate_services(self) -> impl Iterator<Item = StackDescriptor<Self::Item>>
	where
		Self::Item: IdentifyService,
	{
		self.into_group_map_by(|s| s.stack().to_owned())
			.into_iter()
			.map(|(stack, services)| StackDescriptor { stack, services })
	}

	fn match_services<'a, O, F, R>(self, others: O, mut f: F) -> impl Iterator<Item = R> + 'a
	where
		Self: 'a,
		Self::Item: IdentifyService,
		O: Iterator + 'a,
		O::Item: IdentifyService,
		F: FnMut(Self::Item, Option<O::Item>) -> R + 'a,
	{
		fn find_remove<T>(source: &mut Vec<T>, predicate: impl FnMut(&T) -> bool) -> Option<T> {
			let index = source.iter().position(predicate)?;

			Some(source.swap_remove(index))
		}

		let mut others = others.into_group_map_by(|s| s.stack().to_owned());

		self.map(move |s| {
			let other = others
				.get_mut(s.stack())
				.and_then(|o| find_remove(o, |o| o.service() == s.service()));

			f(s, other)
		})
	}
}

impl<T: Iterator> FilterIterExt for T {}

pub trait FilterService {
	fn filter(&self, service: &impl IdentifyService) -> bool;
}

pub trait IdentifyService {
	fn stack(&self) -> &str;
	fn service(&self) -> &str;
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
}

impl FromStr for ServiceDescriptor {
	type Err = anyhow::Error;

	fn from_str(s: &str) -> Result<Self, Self::Err> {
		let filter = ServiceFilter::from_str(s)?;

		Ok(Self {
			stack: filter.stack,
			service: filter.service.ok_or_else(|| anyhow!("no service specified"))?,
		})
	}
}

#[derive(Debug, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub struct StackDescriptor<T> {
	pub stack: String,
	pub services: Vec<T>,
}

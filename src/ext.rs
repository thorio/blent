use itertools::Either;
use std::convert::identity;

pub trait IntoEither: Sized {
	fn either<T>(self, is_left: bool, left: impl FnOnce(Self) -> T) -> Either<T, Self> {
		self.either_or(is_left, left, identity)
	}

	fn either_or<L, R>(
		self,
		is_left: bool,
		left: impl FnOnce(Self) -> L,
		right: impl FnOnce(Self) -> R,
	) -> Either<L, R> {
		match is_left {
			true => Either::Left(left(self)),
			false => Either::Right(right(self)),
		}
	}

	fn either_with<T, A>(self, arg: Option<A>, left: impl FnOnce(Self, A) -> T) -> Either<T, Self> {
		self.either_or_with(arg, left, identity)
	}

	fn either_or_with<L, R, A>(
		self,
		arg: Option<A>,
		left: impl FnOnce(Self, A) -> L,
		right: impl FnOnce(Self) -> R,
	) -> Either<L, R> {
		match arg {
			Some(v) => Either::Left(left(self, v)),
			None => Either::Right(right(self)),
		}
	}
}

impl<T> IntoEither for T {}

pub trait EitherExt<T> {
	fn unwrap(self) -> T;
}

impl<T> EitherExt<T> for Either<T, T> {
	/// Unwraps either side of the `Either`, if both sides have identical types.
	/// always returns a value.
	fn unwrap(self) -> T {
		match self {
			Self::Left(l) => l,
			Self::Right(r) => r,
		}
	}
}

pub trait IterExt: Iterator + Sized {
	/// Returns an iterator adapter with only the `Result::Ok` values,
	/// calling the provided closure on every `Result::Err` value.
	fn filter_err_and<T, E>(self, mut f: impl FnMut(E)) -> impl Iterator<Item = T>
	where
		Self: Iterator<Item = Result<T, E>>,
	{
		self.filter_map(move |r| match r {
			Ok(v) => Some(v),
			Err(e) => {
				f(e);
				None
			}
		})
	}

	fn filter_count(self, predicate: impl FnMut(&Self::Item) -> bool) -> usize {
		self.filter(predicate).count()
	}
}

impl<T: Iterator> IterExt for T {}

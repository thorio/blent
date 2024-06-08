use itertools::Either;

pub trait IntoEither {
	fn either<T>(self, is_left: bool, left: impl FnOnce(Self) -> T) -> Either<T, Self>
	where
		Self: Sized,
	{
		self.either_or(is_left, left, |s| s)
	}

	fn either_or<L, R>(self, is_left: bool, left: impl FnOnce(Self) -> L, right: impl FnOnce(Self) -> R) -> Either<L, R>
	where
		Self: Sized,
	{
		if is_left {
			Either::Left(left(self))
		} else {
			Either::Right(right(self))
		}
	}
}

impl<T> IntoEither for T {}

pub trait IterExt {
	/// Returns an iterator adapter with only the `Result::Ok` values,
	/// calling the provided closure on every `Result::Err` value.
	fn filter_err_and<T, E>(self, f: impl Fn(E)) -> impl Iterator<Item = T>
	where
		Self: Iterator<Item = Result<T, E>> + Sized,
	{
		self.filter_map(move |r| match r {
			Ok(v) => Some(v),
			Err(e) => {
				f(e);
				None
			}
		})
	}
}

impl<T: Iterator> IterExt for T {}

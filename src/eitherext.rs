use either::Either;

pub trait EitherExt {
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

impl<T> EitherExt for T {}

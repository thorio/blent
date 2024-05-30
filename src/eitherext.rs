use either::Either;

pub trait EitherExt {
	fn either_left<T>(self, is_left: bool, left: impl FnOnce(Self) -> T) -> Either<T, Self>
	where
		Self: Sized,
	{
		self.either_or(is_left, left, |s| s)
	}

	fn either_right<T>(self, is_right: bool, right: impl FnOnce(Self) -> T) -> Either<Self, T>
	where
		Self: Sized,
	{
		self.either_or(is_right, |s| s, right)
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

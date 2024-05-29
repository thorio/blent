pub trait IteratorExt: Iterator {
	fn when<'a, T>(self, condition: bool, if_true: impl FnOnce(Self) -> T) -> Box<dyn Iterator<Item = Self::Item> + 'a>
	where
		Self: Sized + 'a,
		T: Iterator<Item = Self::Item> + 'a,
	{
		self.when_else(condition, if_true, |i| i)
	}

	fn when_else<'a, T, F, R>(
		self,
		condition: bool,
		if_true: impl FnOnce(Self) -> T,
		if_false: impl FnOnce(Self) -> F,
	) -> Box<dyn Iterator<Item = R> + 'a>
	where
		Self: Sized + 'a,
		T: Iterator<Item = R> + 'a,
		F: Iterator<Item = R> + 'a,
	{
		match condition {
			true => Box::new(if_true(self)),
			false => Box::new(if_false(self)),
		}
	}
}

impl<T: Iterator> IteratorExt for T {}

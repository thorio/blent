using System;

namespace Blent.Utility
{
	/// <summary>
	/// <see cref="IProgress{T}"/> implementation without any synchronization. <br />
	/// <see cref="Report(T)"/> simply executes the <see cref="Action{T}"/> passed in the constructor.
	/// </summary>
	public class CustomProgress<T> : IProgress<T>
	{
		private readonly Action<T> _action;

		public CustomProgress(Action<T> action) =>
			_action = action;

		public void Report(T value) =>
			_action(value);
	}
}

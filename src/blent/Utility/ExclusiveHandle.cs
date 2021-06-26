using System;

namespace Blent.Utility
{
	public class ExclusiveHandle<T> : IDisposable
	{
		private bool _disposed;
		private readonly T _value;
		private Action _onRelease;

		public ExclusiveHandle(T value, Action onRelease)
		{
			_value = value;
			_onRelease = onRelease;
		}

		public T Value => _disposed ? throw new ObjectDisposedException(null) : _value;

		public void Dispose()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("Object was already disposed");
			}
			_disposed = true;
			_onRelease();
		}
	}
}

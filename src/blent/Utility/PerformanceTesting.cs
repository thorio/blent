using System.Diagnostics;

namespace Blent.Utility
{
	public static class PerformanceTesting
	{
		private static Stopwatch _stopwatch;
		private static long _previous;

		public static void Begin()
		{
#if CHECKPERF
			_stopwatch = Stopwatch.StartNew();
			_previous = 0;
			Checkpoint("Begin");
#endif
		}

		public static void Checkpoint(string message)
		{
#if CHECKPERF
			var elapsed = _stopwatch.ElapsedMilliseconds;
			Console.Error.WriteLine($"PERF | {elapsed,5}ms | +{elapsed - _previous,-5} | {message}");
			_previous = elapsed;
#endif
		}
	}
}

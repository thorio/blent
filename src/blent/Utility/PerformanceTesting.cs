using System.Diagnostics;

namespace Blent.Utility
{
	public static class PerformanceTesting
	{
		private static Stopwatch _stopwatch;
		private static long _previous;
		private static bool _enable;

		public static long GetElapsedMilliseconds() =>
			_stopwatch.ElapsedMilliseconds;

		public static void Begin(bool printCheckpoints)
		{
			_stopwatch = Stopwatch.StartNew();

			if (printCheckpoints) {
				_previous = 0;
				_enable = true;
				Checkpoint("Begin");
			}
		}

		public static void Checkpoint(string message)
		{
			if (!_enable) return;

			var elapsed = GetElapsedMilliseconds();
			Output.Basic.WriteLine($"PERF | {elapsed,5}ms | +{elapsed - _previous,-5} | {message}");
			_previous = elapsed;
		}
	}
}

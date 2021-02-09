using Blent.Utility.Drawing;
using System.Diagnostics;

namespace Blent.Utility
{
	public static class PerformanceTesting
	{
		private static Stopwatch _stopwatch;
		private static long _previous;
		private static bool _enable;
		
		public static void Begin()
		{
			_stopwatch = Stopwatch.StartNew();
			_previous = 0;
			_enable = true;
			Checkpoint("Begin");
		}

		public static void Checkpoint(string message)
		{
			if (!_enable) return;

			var elapsed = _stopwatch.ElapsedMilliseconds;
			Output.Error.WriteLine($"PERF | {elapsed,5}ms | +{elapsed - _previous,-5} | {message}");
			_previous = elapsed;
		}
	}
}

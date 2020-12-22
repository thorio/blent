using Blent.Utility.Drawing;
using System;

namespace Blent.Utility
{
	public static class ErrorHandling
	{
		public static void LogWarn(string message) =>
			Log(message, "WARN", Color.Warning);

		public static void LogError(string message) =>
			Log(message, "ERROR", Color.Danger);

		public static void LogFatalAndQuit(string message)
		{
			Log(message, "FATAL", Color.Danger);
			PerformanceTesting.Checkpoint("Fatal");
			Environment.Exit(1);
		}

		/// <summary>
		/// Returns some value to make the compiler happy, even though the runtime will never actually get that far.
		/// </summary>
		public static T LogFatalAndQuit<T>(string message)
		{
			LogFatalAndQuit(message);
			return default;
		}

		public static void LogException(Exception ex)
		{
			Output.Error.Write("UNHANDLED EXCEPTION: ", Color.Danger);
			Output.Error.WriteLine($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n");
			Output.Error.WriteLine($"Please report this issue at {Constants.IssuesUrl}", Color.Warning);
		}

		private static void Log(string message, string level, Color color)
		{
			Output.Error.Write($"{level}: ", color);
			Output.Error.WriteLine(message);
		}
	}
}

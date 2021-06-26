using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;

namespace Blent.Utility
{
	public static class ErrorPrinter
	{
		public static void Warn(string message) =>
			Print(message, LogLevel.Warn, Color.Warning);

		public static void Error(string message) =>
			Print(message, LogLevel.Error, Color.Danger);

		public static void Fatal(string message)
		{
			Print(message, LogLevel.Fatal, Color.Danger);
		}

		public static void UnhandledException(Exception ex)
		{
			Output.Error.Write("UNHANDLED EXCEPTION: ", Color.Danger);
			Output.Error.WriteLine(FormatException(ex));

			if (ex is AggregateException aggregateException)
			{
				Output.Error.WriteLine("Inner Exceptions:");
				foreach (var innerException in aggregateException.InnerExceptions)
				{
					Output.Error.WriteLine(FormatException(innerException));
				}
			}
			Output.Error.WriteLine($"Please report this issue at {Constants.IssuesUrl}", Color.Warning);
		}

		public static void HandledException(string message, Exception ex)
		{
			Output.Error.Write("EXCEPTION: ", Color.Danger);
			Output.Error.WriteLine(message);

			Output.Error.WriteLine(FormatException(ex, false));

			if (ex is AggregateException aggregateException)
			{
				Output.Error.WriteLine("Inner Exceptions:");
				foreach (var innerException in aggregateException.InnerExceptions)
				{
					Output.Error.WriteLine(FormatException(innerException, false));
				}
			}
		}

		private static string FormatException(Exception ex, bool showStackTrace = true)
		{
			var trace = showStackTrace ? ex.StackTrace + "\n" : "";
			return $"{ex.GetType().FullName}: {ex.Message}\n{trace}";
		}

		private static void Print(string message, LogLevel level, Color color)
		{
			Output.Error.Write($"{level.ToString().ToUpper()}: ", color);
			Output.Error.WriteLine(message);
		}
	}
}

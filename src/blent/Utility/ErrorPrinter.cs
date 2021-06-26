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
			Output.Fancy.Write("UNHANDLED EXCEPTION: ", Color.Danger);
			Output.Fancy.WriteLine(FormatException(ex));

			if (ex is AggregateException aggregateException)
			{
				Output.Fancy.WriteLine("Inner Exceptions:");
				foreach (var innerException in aggregateException.InnerExceptions)
				{
					Output.Fancy.WriteLine(FormatException(innerException));
				}
			}
			Output.Fancy.WriteLine($"Please report this issue at {Constants.IssuesUrl}", Color.Warning);
		}

		public static void HandledException(string message, Exception ex)
		{
			Output.Fancy.Write("EXCEPTION: ", Color.Danger);
			Output.Fancy.WriteLine(message);

			Output.Fancy.WriteLine(FormatException(ex, false));

			if (ex is AggregateException aggregateException)
			{
				Output.Fancy.WriteLine("Inner Exceptions:");
				foreach (var innerException in aggregateException.InnerExceptions)
				{
					Output.Fancy.WriteLine(FormatException(innerException, false));
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
			Output.Fancy.Write($"{level.ToString().ToUpper()}: ", color);
			Output.Fancy.WriteLine(message);
		}
	}
}

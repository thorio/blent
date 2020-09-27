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
			System.Environment.Exit(1);
		}

		/// <summary>
		/// Returns some value to make the compiler happy, even though the runtime will never actually get that far.
		/// </summary>
		public static T LogFatalAndQuit<T>(string message)
		{
			LogFatalAndQuit(message);
			return default;
		}

		private static void Log(string message, string level, Color color)
		{
			Output.Error.Write($"{level}: ", color);
			Output.Error.WriteLine(message);
		}
	}
}

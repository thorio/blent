using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;

namespace Blent.Utility
{
	public static class Output
	{
		static Output()
		{
			// provide default configuration, useful when something goes wrong in parsing
			Out = new OutputWriter(Console.Out);
			Error = new OutputWriter(Console.Error);
			Logger = new LogfmtLogger(null, LogLevel.None);
		}

		public static OutputWriter Out { get; private set; }
		public static OutputWriter Error { get; private set; }
		public static ILogger Logger { get; private set; }

		public static void Init(OutputMode mode, LogLevel level, string verbName)
		{
			Out.Enabled = Error.Enabled = mode == OutputMode.Fancy;

			if (mode == OutputMode.Logfmt)
			{
				Logger = new LogfmtLogger(new TextWriterLogTarget(Console.Out), level, new { verb = verbName });
			}
		}
	}
}

using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;

namespace Blent.Utility
{
	public static class Output
	{
		private static FileLogTarget _fileLogTarget;

		static Output()
		{
			// provide default configuration, useful when something goes wrong in parsing
			Basic = new BasicOutputWriter(new TextWriterOutputTarget(Console.Out));
			Fancy = new OutputWriter(new TextWriterOutputTarget(Console.Error));
			Logger = new LogfmtLogger(null, LogLevel.None);
		}

		/// <summary>
		/// Writes to <see cref="Console.Out"/> and only prints text.<br/>
		/// Only use for machine-readable or very basic output.
		/// </summary>
		public static BasicOutputWriter Basic { get; private set; }

		/// <summary>
		/// Writes to <see cref="Console.Error"/>, allows for color and other fancy output.<br/>
		/// Use for most output.
		/// </summary>
		public static OutputWriter Fancy { get; private set; }

		/// <summary>
		/// Default logger, only use when no other logger is available.
		/// </summary>
		public static ILogger Logger { get; private set; }

		public static void Init(OutputMode mode, LogLevel level, string logFile, string verbName)
		{
			Basic.Enabled = Fancy.Enabled = mode == OutputMode.Fancy;

			var logTarget = InitLogTarget(mode, logFile);

			if (logTarget.TargetCount > 0)
			{
				Logger = new LogfmtLogger(logTarget, level, new { verb = verbName });
			}
		}

		public static void Dispose()
		{
			_fileLogTarget?.Dispose();
		}

		private static MultiLogTarget InitLogTarget(OutputMode mode, string logFile)
		{
			var logTarget = new MultiLogTarget();

			if (mode == OutputMode.Logfmt)
			{
				logTarget.AddTarget(new TextWriterLogTarget(Console.Out));
			}

			if (!string.IsNullOrWhiteSpace(logFile))
			{
				_fileLogTarget = new FileLogTarget(logFile);
				logTarget.AddTarget(_fileLogTarget);
			}

			return logTarget;
		}
	}
}

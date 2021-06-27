using Blent.Utility;
using Blent.Utility.Logging;
using CommandLine;

namespace Blent.Verb
{
	public abstract class Options : IOptions
	{
		public string PassthroughArguments { get; set; }

		[Option("app-directory", HelpText = "Set the app directory.")]
		public string AppDirectory { get; set; }

		[Option("log-level", Default = LogLevel.Info, HelpText = "Set the minimum log level.")]
		public LogLevel LogLevel { get; set; }

		[Option("output-mode", Default = OutputMode.Fancy, HelpText = "Set the output mode.")]
		public OutputMode OutputMode { get; set; }

		[Option("log-file", Default = null, HelpText = "Set the logfile path.")]
		public string LogFile { get; set; }
	}
}

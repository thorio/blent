using Blent.Configuration;
using Blent.Configuration.Models;
using Blent.Utility;
using Blent.Utility.Logging;
using Blent.Verb;

namespace Blent.Startup
{
	public static class VerbExecuter
	{
		public static void ExecuteVerb(IVerb verb, IOptions options)
		{
			var logger = Init(verb, options);

			logger.Debug("executing verb");

			PerformanceTesting.Checkpoint("Begin Verb");
			verb.Execute(options, Output.Logger);

			logger.Info("execution completed", new { time = PerformanceTesting.GetElapsedMilliseconds() });

			Output.Dispose();
		}

		private static ILogger Init(IVerb verb, IOptions options)
		{
			var logger = InitLogger(options, verb.GetVerbName());

			Settings.InitUserConfig(logger);

			ProcessGlobalArguments(logger, options, Settings.UserConfig);

			PreRunChecks.PerformChecks(Output.Logger, verb, Settings.UserConfig);

			return logger;
		}

		private static ILogger InitLogger(IOptions options, string verbName)
		{
			Output.Init(options.OutputMode, options.LogLevel, options.LogFile, verbName);
			Output.Logger.Debug("logger initialized", new { output_mode = options.OutputMode, log_level = options.LogLevel });

			Output.Logger.Info("application invoked", new
			{
				args = string.Join(' ', System.Environment.GetCommandLineArgs()),
				name = AssemblyInfo.GetName(),
				version = AssemblyInfo.GetVersion(),
				commit = Constants.CommitHash,
			});

			return Output.Logger;
		}

		private static void ProcessGlobalArguments(ILogger logger, IOptions options, UserConfig config)
		{
			if (options.AppDirectory != null)
			{
				logger.Trace("setting app directory from commandline", new { app_directory = options.AppDirectory });
				config.AppDirectory = options.AppDirectory;
			}
		}
	}
}

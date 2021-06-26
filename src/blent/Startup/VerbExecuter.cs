using Blent.Configuration;
using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Logging;
using Blent.Verb;

namespace Blent.Startup
{
	public static class VerbExecuter
	{
		public static void ExecuteVerb(IVerb verb, IOptions options)
		{
			ProcessGlobalArguments(options, verb.GetVerbName());
			PerformChecks(Output.Logger, verb);

			Output.Logger.Debug("executing verb", new {
				args = string.Join(' ', System.Environment.GetCommandLineArgs()),
				name = AssemblyInfo.GetName(),
				version = AssemblyInfo.GetVersion(),
				commit = AssemblyInfo.GetShortCommitHash(),
			});

			PerformanceTesting.Checkpoint("Begin Verb");
			verb.Execute(options, Output.Logger);
		}

		public static void ProcessGlobalArguments(IOptions options, string verbName)
		{
			Output.Init(options.OutputMode, options.LogLevel, verbName);
			Output.Logger.Debug("logger initialized");

			if (options.AppDirectory != null)
			{
				Output.Logger.Trace("setting appdirectory from commandline", new { app_directory = options.AppDirectory });
				Settings.SetAppDirectory(options.AppDirectory);
			}
		}

		public static void PerformChecks(ILogger logger, IVerb verb)
		{
			PerformanceTesting.Checkpoint("Begin Checks");
			logger.Trace("checking docker");
			if (verb.RequiresDocker && Settings.GetUserConfig().Checks.Docker && !Docker.IsRunning())
			{
				logger.Fatal("docker check failed");
				ErrorPrinter.FatalAndQuit("Docker daemon is unreachable or not running.");
			}
			PerformanceTesting.Checkpoint("End Docker Check");
		}
	}
}

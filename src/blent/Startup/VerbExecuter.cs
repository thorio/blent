using Blent.Configuration;
using Blent.Interop;
using Blent.Utility;
using Blent.Verb;

namespace Blent.Startup
{
	public static class VerbExecuter
	{
		public static void ExecuteVerb(IVerb verb, IOptions options)
		{
			ProcessGlobalArguments(options);
			PerformChecks(verb);
			PerformanceTesting.Checkpoint("Begin Verb");
			verb.Execute(options);
		}

		public static void ProcessGlobalArguments(IOptions options)
		{
			if (options.AppDirectory != null)
			{
				Settings.SetAppDirectory(options.AppDirectory);
			}
		}

		public static void PerformChecks(IVerb verb)
		{
			PerformanceTesting.Checkpoint("Begin Checks");
			if (verb.RequiresDocker && Settings.GetUserConfig().Checks.Docker && !Docker.IsRunning())
			{
				ErrorHandling.LogFatalAndQuit("Docker daemon is unreachable or not running.");
			}
			PerformanceTesting.Checkpoint("End Docker Check");
		}
	}
}

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
			verb.Execute(options);
		}

		public static void ProcessGlobalArguments(IOptions options)
		{
			if (options.AppDirectory != null)
			{
				Configuration.Settings.AppDirectory = options.AppDirectory;
			}
		}

		public static void PerformChecks(IVerb verb)
		{
			if (verb.RequiresDocker && !Docker.IsRunning())
			{
				ErrorHandling.LogFatalAndQuit("Docker daemon is unreachable or not running.");
			}
			if (verb.RequiresDocker && !DockerCompose.IsInstalled())
			{
				ErrorHandling.LogFatalAndQuit("Docker-compose is not installed.");
			}
		}
	}
}

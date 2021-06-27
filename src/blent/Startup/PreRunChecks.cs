using Blent.Configuration;
using Blent.Configuration.Models;
using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Logging;
using Blent.Verb;
using System.IO;
using System.Linq;

namespace Blent.Startup
{
	public static class PreRunChecks
	{
		public static void PerformChecks(ILogger logger, IVerb verb, UserConfig config)
		{
			PerformanceTesting.Checkpoint("Begin Checks");

			CheckPlatform(logger);
			CheckAppDirectory(logger);
			CheckDocker(logger, verb, config);
		}

		private static void CheckPlatform(ILogger logger)
		{
			logger.Trace("checking platform");

			var supportedPlatforms = new[] { Platform.Linux, Platform.Windows };
			var platform = Environment.GetCurrentPlatform();

			if (!supportedPlatforms.Contains(platform))
			{
				logger.Warn("the current platform is not supported, proceed with caution", new { platform });
				ErrorPrinter.Warn($"The current platform ({platform}) is not supported, proceed with caution.");
			}

			PerformanceTesting.Checkpoint("End Platform Check");
		}

		private static void CheckAppDirectory(ILogger logger)
		{
			logger.Trace("checking app directory");

			var path = Settings.GetAppDirectory();
			if (!Directory.Exists(path))
			{
				logger.Fatal("app directory does not exist", new { path });
				throw new FatalException($"App directory [{path}] does not exist");
			};

			PerformanceTesting.Checkpoint("End App Directory Check");
		}

		private static void CheckDocker(ILogger logger, IVerb verb, UserConfig config)
		{
			logger.Trace("checking docker");

			if (verb.RequiresDocker && config.Checks.Docker && !Docker.IsRunning())
			{
				logger.Fatal("docker check failed");
				throw new FatalException("Docker daemon is unreachable or not running.");
			}

			PerformanceTesting.Checkpoint("End Docker Check");
		}
	}
}

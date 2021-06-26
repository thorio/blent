using Blent.Startup;
using Blent.Utility;
using System;
using System.Linq;

namespace Blent
{
	public class Program
	{
		public static int Main(string[] args)
		{
			try
			{
				PreRun();
				return Run(args);
			}
			catch (Exception ex)
			{
				PerformanceTesting.Checkpoint("Unhandled Exception");

				Output.Logger.Fatal(null, ex);
				ErrorPrinter.UnhandledException(ex);

				return 1;
			}
			finally
			{
				Output.Error.ResetStyling();
				PerformanceTesting.Checkpoint("End");
			}
		}

		private static void PreRun()
		{
			if (Environment.GetEnvironmentVariable("BLENT_PERF_TEST") == "true")
			{
				PerformanceTesting.Begin();
			}
		}

		private static int Run(string[] args)
		{
			var errors = new CommandLineParser(args).ParseAndExecuteVerb();
			return errors.Any() ? 1 : 0;
		}
	}
}

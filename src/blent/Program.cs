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
				return HandleException(ex);
			}
			finally
			{
				Output.Fancy.ResetStyling();
				PerformanceTesting.Checkpoint("End");
			}
		}

		private static int HandleException(Exception ex)
		{
			if (ex is FatalException fatalEx)
			{
				PerformanceTesting.Checkpoint("Fatal Exception");
				ErrorPrinter.Fatal(fatalEx.Message);

				if (fatalEx.InnerException != null)
				{
					ErrorPrinter.HandledException(null, fatalEx.InnerException);
				}

				return fatalEx.ExitCode;
			}

			PerformanceTesting.Checkpoint("Unhandled Exception");

			Output.Logger.Fatal(null, ex);
			ErrorPrinter.UnhandledException(ex);

			return 1;
		}

		private static void PreRun()
		{
			var printPerformanceCheckpoints = Environment.GetEnvironmentVariable("BLENT_PERF_TEST") == "true";
			PerformanceTesting.Begin(printPerformanceCheckpoints);
		}

		private static int Run(string[] args)
		{
			var errors = new CommandLineParser(args).ParseAndExecuteVerb();
			return errors.Any() ? 1 : 0;
		}
	}
}

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
				PerformanceTesting.Begin();
				var errors = new CommandLineParser(args).ParseAndExecuteVerb();
				return errors.Any() ? 1 : 0;
			}
			catch (Exception ex)
			{
				PerformanceTesting.Checkpoint("Unhandled Exception");
				ErrorHandling.LogException(ex);
				return 1;
			}
			finally
			{
				PerformanceTesting.Checkpoint("End");
			}
		}
	}
}

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
				var errors = new CommandLineParser(args).ParseAndExecuteVerb();
				return errors.Any() ? 1 : 0;
			}
			catch (Exception ex)
			{
				ErrorHandling.LogException(ex);
				return 1;
			}
		}
	}
}

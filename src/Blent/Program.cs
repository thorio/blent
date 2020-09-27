using Blent.CommandLineParser;
using Blent.Utility;
using System;
using System.Linq;

namespace Blent
{
	public class Program
	{
		private const string IssueUrl = "https://github.com/thorio/blent/issues";

		public static int Main(string[] args)
		{
			try
			{
				var errors = new Parser(args).ParseAndExecuteVerb();
				return errors.Any() ? 1 : 0;
			}
			catch (Exception ex)
			{
				Output.Error.Write("UNHANDLED EXCEPTION: ", Color.Danger);
				Output.Error.WriteLine($"{ex.Message}\n{ex.StackTrace}\n");
				Output.Error.WriteLine($"Please report this issue at {IssueUrl}", Color.Warning);
				return 1;
			}
		}
	}
}

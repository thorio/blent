using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Logs
{
	[Verb("logs", HelpText = "Print logs of a stack.")]
	public class LogsOptions : Options
	{
		[Value(0, Required = true, HelpText = "Stack to fetch logs for.", MetaName = "STACK")]
		public string Stacks { get; set; }

		[Value(1, Required = false, HelpText = "Service(s) to fetch logs for.", MetaName = "SERVICE...")]
		public IEnumerable<string> Services { get; set; }

		[Option('f', "follow", HelpText = "Follow log output.")]
		public bool Follow { get; set; }

		[Option('t', "timestamps", HelpText = "Show timestamps.")]
		public bool ShowTimestamps { get; set; }
	}
}

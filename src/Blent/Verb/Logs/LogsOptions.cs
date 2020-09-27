using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Logs
{
	[Verb("logs", HelpText = "Fetch the logs of a project.")]
	public class LogsOptions : Options
	{
		[Value(0, Required = true, HelpText = "Project to fetch logs for.", MetaName = "project")]
		public string Project { get; set; }

		[Value(1, Required = false, HelpText = "Service(s) to fetch logs for.", MetaName = "service(s)")]
		public IEnumerable<string> Services { get; set; }

		[Option('f', "follow", HelpText = "Follow log output.")]
		public bool Follow { get; set; }

		[Option('t', "timestamps", HelpText = "Show timestamps.")]
		public bool ShowTimestamps { get; set; }
	}
}

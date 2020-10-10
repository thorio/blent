using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Up
{
	[Verb("up", HelpText = "Start a project.")]
	public class UpOptions : Options
	{
		[Value(0, HelpText = "Project(s) to start. Leave blank to start all stopped projects.", MetaName = "PROJECT...")]
		public IEnumerable<string> Projects { get; set; }

		[Option('f', "force", HelpText = "Force container recreation.")]
		public bool ForceRecreate { get; set; }

		[Option('a', "attach", HelpText = "Attach to services.")]
		public bool Attach { get; set; }
	}
}

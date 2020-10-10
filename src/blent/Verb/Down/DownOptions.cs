using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Down
{
	[Verb("down", HelpText = "Stop a project.")]
	public class DownOptions : Options
	{
		[Value(0, HelpText = "Project(s) to stop. Leave blank to stop all running projects.", MetaName = "PROJECT...")]
		public IEnumerable<string> Projects { get; set; }

		[Option('r', "remove-orphans", HelpText = "Remove containers for services not defined in the compose file.")]
		public bool RemoveOrphans { get; set; }
	}
}

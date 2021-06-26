using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Down
{
	[Verb("down", HelpText = "Stop a stack.")]
	public class DownOptions : Options
	{
		[Value(0, HelpText = "Stack(s) to stop. Leave blank to stop all running stacks.", MetaName = "STACK...")]
		public IEnumerable<string> Stacks { get; set; }

		[Option('r', "remove-orphans", HelpText = "Remove containers for services not defined in the compose file.")]
		public bool RemoveOrphans { get; set; }
	}
}

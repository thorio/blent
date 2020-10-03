using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Down
{
	[Verb("down", HelpText = "Stop a project.")]
	public class DownOptions : Options
	{
		[Value(0, Min = 1, HelpText = "Project(s) to Stop.", MetaName = "PROJECT...")]
		public IEnumerable<string> Values { get; set; }
	}
}

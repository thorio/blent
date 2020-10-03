using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Update
{
	[Verb("update", HelpText = "Update containers of running projects.")]
	public class UpdateOptions : Options
	{
		[Value(0, HelpText = "Project(s) to update. Leave blank to update all running projects.", MetaName = "PROJECT...")]
		public IEnumerable<string> Projects { get; set; }

		[Option('r', "remove-dangling", HelpText = "Remove dangling images.")]
		public bool RemoveDanglingImages { get; set; }
	}
}

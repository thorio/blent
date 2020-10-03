using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Update
{
	[Verb("update", HelpText = "Update containers of running projects.")]
	public class UpdateOptions : Options
	{
		[Value(0, HelpText = "Project(s) to Update. Leave blank to update all running projects.", MetaName = "project(s)")]
		public IEnumerable<string> Projects { get; set; }

		[Option('r', "remove-dangling", HelpText = "Also remove dangling images.")]
		public bool RemoveDanglingImages { get; set; }
	}
}

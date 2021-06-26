using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Update
{
	[Verb("update", HelpText = "Update the containers of running stacks.")]
	public class UpdateOptions : Options
	{
		[Value(0, HelpText = "Stack(s) to update. Leave blank to update all running stacks.", MetaName = "STACK...")]
		public IEnumerable<string> Stack { get; set; }

		[Option('r', "remove-dangling", HelpText = "Remove dangling images.")]
		public bool RemoveDanglingImages { get; set; }
	}
}

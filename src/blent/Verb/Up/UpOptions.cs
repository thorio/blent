using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Up
{
	[Verb("up", HelpText = "Start a stack.")]
	public class UpOptions : Options
	{
		[Value(0, HelpText = "Stack(s) to start. Leave blank to start all stopped stacks.", MetaName = "STACK...")]
		public IEnumerable<string> Stacks { get; set; }

		[Option('f', "force", HelpText = "Force container recreation.")]
		public bool ForceRecreate { get; set; }

		[Option('a', "attach", HelpText = "Attach to services.")]
		public bool Attach { get; set; }
	}
}

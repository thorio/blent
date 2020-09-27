using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Up
{
	[Verb("up", HelpText = "Start a project.")]
	public class UpOptions : Options
	{
		[Value(0, Min = 1, HelpText = "Projects to Start")]
		public IEnumerable<string> Values { get; set; }
	}
}

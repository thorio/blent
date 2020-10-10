using CommandLine;

namespace Blent.Verb.Shell
{
	[Verb("shell", HelpText = "Opens an interactive shell in a container.")]
	public class ShellOptions : Options
	{
		[Value(0, Required = true, HelpText = "Project to open a shell in.", MetaName = "PROJECT")]
		public string Project { get; set; }

		[Value(1, Required = true, HelpText = "Service to open a shell in.", MetaName = "SERVICE")]
		public string Service { get; set; }

		[Option('c', "command", Default = "sh", HelpText = "The command to execute.")]
		public string Shell { get; set; }
	}
}

using CommandLine;

namespace Blent.Verb.Version
{
	[Verb("version", HelpText = "Display version and other metadata.")]
	public class VersionOptions : Options
	{
		[Option('s', "short", HelpText = "Print just the version number.")]
		public bool Short { get; set; }
	}
}

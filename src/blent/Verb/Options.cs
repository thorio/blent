using CommandLine;

namespace Blent.Verb
{
	public abstract class Options : IOptions
	{
		public string PassthroughArguments { get; set; }

		[Option("app-directory", HelpText = "Set the app directory.")]
		public string AppDirectory { get; set; }
	}
}

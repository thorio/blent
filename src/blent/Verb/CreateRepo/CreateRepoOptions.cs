using CommandLine;

namespace Blent.Verb.CreateRepo
{
	[Verb("create-repo", HelpText = "Creates the default Repository.")]
	public class CreateRepoOptions : Options
	{
		[Value(0)]
		public string RemoteUrl { get; set; }
	}
}

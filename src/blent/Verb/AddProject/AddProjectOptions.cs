using CommandLine;

namespace Blent.Verb.AddProject
{
	[Verb("add-project", HelpText = "Adds a local project to a repository")]
	public class AddProjectOptions : Options
	{
		[Value(0, Required = true, HelpText = "Name of the local project.", MetaName = "PROJECT")]
		public string Name { get; set; }

		[Value(0, Required = false, HelpText = "Project ID in the repository.", MetaName = "ID")]
		public string Id { get; set; }
	}
}

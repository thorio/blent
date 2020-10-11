using CommandLine;

namespace Blent.Verb.CreateProject
{
	[Verb("create-project", HelpText = "Initializes a new project.")]
	public class CreateProjectOptions : Options
	{
		[Value(0, Required = true, HelpText = "Project to initialize.", MetaName = "PROJECT")]
		public string Name { get; set; }

		[Value(0, Required = false, HelpText = "Project id.", MetaName = "ID")]
		public string Id { get; set; }
	}
}

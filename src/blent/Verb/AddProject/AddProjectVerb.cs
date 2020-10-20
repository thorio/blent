using Blent.Configuration;
using Blent.ProjectRepository;
using System.IO;

namespace Blent.Verb.AddProject
{
	public class AddProjectVerb : Verb<AddProjectOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => "PROJECT [ID]";

		public override void Execute(AddProjectOptions options)
		{
			var repo = RepositoryProvider.GetDefaultRepository();
			var project = repo.AddProject(options.Name, options.Id);
		}
	}
}

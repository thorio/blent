using Blent.Interop;
using Blent.Utility;
using System.Linq;

namespace Blent.Verb.Up
{
	public class UpVerb : Verb<UpOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-up_options]";

		public override void Execute(UpOptions options)
		{
			var projects = options.Projects;
			if (!projects.Any())
			{
				// get all projects on disk, then subtract projects that are already running
				projects = ProjectDirectory.GetProjects()
					.Except(Docker.GetComposeProjects());
			}

			if (!projects.Any())
			{
				ErrorHandling.LogError("There are no stopped projects.");
				return;
			}

			DockerCompose.Up(projects, options.PassthroughArguments, !options.Attach, options.ForceRecreate);
		}
	}
}

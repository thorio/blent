using Blent.Interop;
using Blent.Utility;
using System.Linq;

namespace Blent.Verb.Down
{
	public class DownVerb : Verb<DownOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-down_options]";

		public override void Execute(DownOptions options)
		{
			var projects = options.Projects;
			if (!projects.Any())
			{
				projects = Docker.GetComposeProjects();
			}

			if (!projects.Any())
			{
				ErrorHandling.LogError("There are no running projects.");
				return;
			}

			DockerCompose.Down(projects, options.PassthroughArguments, options.RemoveOrphans);
		}
	}
}

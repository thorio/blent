using Blent.Interop;
using Blent.Utility;
using System.Linq;

namespace Blent.Verb.Update
{
	public class UpdateVerb : Verb<UpdateOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-up_options]";

		public override void Execute(UpdateOptions options)
		{
			var projects = options.Projects;
			if (!projects.Any())
			{
				projects = Docker.GetComposeProjects();
			}

			Output.Out.WriteLine("Pulling images ...", Color.Info);
			DockerCompose.Pull(projects);

			Output.Out.WriteLine("\nRestarting Projects ...", Color.Info);
			DockerCompose.Up(projects, options.PassthroughArguments);

			if (options.RemoveDanglingImages)
			{
				RemoveDanglingImages();
			}
		}

		private void RemoveDanglingImages()
		{
			Output.Out.WriteLine("\nRemoving dangling images ...", Color.Info);
			var images = Docker.GetImages("dangling=true");
			if (images.Any())
			{
				Docker.RemoveImages(images);
				Output.Out.WriteLine($"Removed {images.Count()} images.");
			}
			else
			{
				Output.Out.WriteLine("No dangling images found.");
			}
		}
	}
}

using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blent.Verb.Update
{
	public class UpdateVerb : Verb<UpdateOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-up_options]";

		public override void Execute(UpdateOptions options)
		{
			var projects = options.Projects.Any() ? options.Projects : Docker.GetComposeProjects();
			if (!projects.Any())
			{
				ErrorHandling.LogError("There are no running projects.");
				return;
			}

			new ParallelTaskManager<string, (TaskState, TaskState)>(projects, GetRow, (project, progress) => Execute(project, progress, options), HandleProgress, new[] { 0, 5, 0 }, new[] { "Project", "Pull", "Restart" })
				.Execute();

			if (options.RemoveDanglingImages)
			{
				RemoveDanglingImages();
			}
		}

		private IEnumerable<string> GetRow(string project) =>
			new[] { project, TaskState.Pending.ToCell().Text, "" };

		public void Execute(string project, IProgress<(TaskState, TaskState)> progress, UpdateOptions options)
		{
			var pullResults = DockerCompose.Pull(project);
			var pullSuccess = pullResults.ExitCode == 0;
			progress.Report((pullSuccess.ToTaskState(), TaskState.Pending));

			if (!pullSuccess) return;

			var upResults = DockerCompose.Up(project, printOutput: false);
			var upSuccess = upResults.ExitCode == 0;
			progress.Report((TaskState.Success, upSuccess.ToTaskState()));
		}

		private void HandleProgress((TaskState pullState, TaskState upState) report, TableRow row)
		{
			row.SetCell(report.pullState.ToCell(), 1);
			row.SetCell(report.upState.ToCell(), 2);
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

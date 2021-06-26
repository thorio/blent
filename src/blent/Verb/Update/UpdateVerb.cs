using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blent.Verb.Update
{
	public class UpdateVerb : Verb<UpdateOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-up_options]";

		public override void Execute(UpdateOptions options, ILogger logger)
		{
			var projects = options.Projects.Any() ? options.Projects : Docker.GetComposeProjects();
			if (!projects.Any())
			{
				logger.Error("no running projects found");
				ErrorPrinter.Error("There are no running projects.");
				return;
			}

			logger.Trace("updating projects in parallel", new { project_count = projects.Count(), projects = string.Join(", ", projects) });

			new ParallelTaskManager<string, (TaskState, TaskState)>(projects, GetRow, (project, progress) => Execute(project, progress, options, logger), HandleProgress,
				new[] { 0, 5, 0 }, new[] { "Project", "Pull", "Restart" })
				.Execute();

			if (options.RemoveDanglingImages)
			{
				RemoveDanglingImages(logger);
			}
		}

		private IEnumerable<string> GetRow(string project) =>
			new[] { project, TaskState.Pending.ToCell().Text, "" };

		public void Execute(string project, IProgress<(TaskState, TaskState)> progress, UpdateOptions options, ILogger logger)
		{
			logger.Trace("pulling project", new { project });

			var pullResults = DockerCompose.Pull(project);
			var pullSuccess = pullResults.ExitCode == 0;
			progress.Report((pullSuccess.ToTaskState(), TaskState.Pending));

			if (!pullSuccess)
			{
				logger.Error("pulling project failed", new { project });
				logger.Debug("pulling project failed", new { project, compose_stdout = pullResults.Output, compose_stderr = pullResults.Error });
				return;
			}

			logger.Trace("recreating project", new { project });

			var upResults = DockerCompose.Up(project, printOutput: false);
			var upSuccess = upResults.ExitCode == 0;
			progress.Report((TaskState.Success, upSuccess.ToTaskState()));

			if (!upSuccess)
			{
				logger.Error("recreating project failed", new { project });
				logger.Debug("recreating project failed", new { project, compose_stdout = upResults.Output, compose_stderr = upResults.Error });
				return;
			}

			logger.Info("recreated project", new { project });
		}

		private void HandleProgress((TaskState pullState, TaskState upState) report, TableRow row)
		{
			row.SetCell(report.pullState.ToCell(), 1);
			row.SetCell(report.upState.ToCell(), 2);
		}

		private void RemoveDanglingImages(ILogger logger)
		{
			logger.Trace("removing dangling images");
			Output.Fancy.WriteLine("\nRemoving dangling images ...", Color.Info);

			var images = Docker.GetImages("dangling=true");
			if (images.Any())
			{
				Docker.RemoveImages(images);
				Output.Fancy.WriteLine($"Removed {images.Count()} images.");
				logger.Info("dangling images removed", new { count = images.Count() });
			}
			else
			{
				logger.Debug("no dangling images found");
				Output.Fancy.WriteLine("No dangling images found.");
			}
		}
	}
}

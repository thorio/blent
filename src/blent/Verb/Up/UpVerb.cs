using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blent.Verb.Up
{
	public class UpVerb : Verb<UpOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-up_options]";

		public override void Execute(UpOptions options, ILogger logger)
		{
			var projects = options.Projects.Any() ? options.Projects : ProjectDirectory.GetProjects().Except(Docker.GetComposeProjects());
			if (!projects.Any())
			{
				logger.Error("no stopped projects found");
				ErrorPrinter.Error("There are no stopped projects.");
				return;
			}

			logger.Trace("starting projects in parallel", new { project_count = projects.Count(), projects = string.Join(", ", projects) });

			new ParallelTaskManager<string, TaskState>(projects, GetRow, (project, progress) => Execute(project, progress, options, logger), HandleProgress, new[] { 0, 5 })
				.Execute();
		}

		private IEnumerable<string> GetRow(string project) =>
			new[] { project, TaskState.Pending.ToCell().Text };

		public void Execute(string project, IProgress<TaskState> progress, UpOptions options, ILogger logger)
		{
			logger.Trace("starting project", new { project });

			var results = DockerCompose.Up(project, options.PassthroughArguments, !options.Attach, options.ForceRecreate, false);
			if (results.ExitCode == 0)
			{
				logger.Info("started project", new { project });
				progress.Report(TaskState.Success);
			}
			else
			{
				logger.Error("starting project failed", new { project });
				logger.Debug("starting project failed", new { project, compose_stdout = results.Output, compose_stderr = results.Error });
				progress.Report(TaskState.Failure);
			}
		}

		private void HandleProgress(TaskState taskState, TableRow row)
		{
			row.SetCell(taskState.ToCell(), 1);
		}
	}
}

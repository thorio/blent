using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blent.Verb.Down
{
	public class DownVerb : Verb<DownOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-down_options]";

		public override void Execute(DownOptions options, ILogger logger)
		{
			var projects = options.Projects.Any() ? options.Projects : Docker.GetComposeProjects();
			if (!projects.Any())
			{
				logger.Error("no running projects found");
				ErrorPrinter.Error("There are no running projects.");
				return;
			}

			logger.Trace("stopping projects in parallel", new { project_count = projects.Count(), projects = string.Join(", ", projects) });

			new ParallelTaskManager<string, TaskState>(projects, GetRow, (project, progress) => Execute(project, progress, options, logger), HandleProgress, new[] { 0, 5 })
				.Execute();
		}

		private IEnumerable<string> GetRow(string project) =>
			new[] { project, TaskState.Pending.ToCell().Text };

		public void Execute(string project, IProgress<TaskState> progress, DownOptions options, ILogger logger)
		{
			logger.Trace("stopping project", new { project });

			var results = DockerCompose.Down(project, options.PassthroughArguments, options.RemoveOrphans, false);
			if (results.ExitCode == 0)
			{
				logger.Info("stopped project", new { project });
				progress.Report(TaskState.Success);
			}
			else
			{
				logger.Error("stopping project failed", new { project });
				logger.Debug("stopping project failed", new { project, compose_stdout = results.Output, compose_stderr = results.Error });
				progress.Report(TaskState.Failure);
			}
		}

		private void HandleProgress(TaskState taskState, TableRow row)
		{
			row.SetCell(taskState.ToCell(), 1);
		}
	}
}

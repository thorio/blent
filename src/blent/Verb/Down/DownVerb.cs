using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blent.Verb.Down
{
	public class DownVerb : Verb<DownOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-down_options]";

		public override void Execute(DownOptions options)
		{
			var projects = options.Projects.Any() ? options.Projects : Docker.GetComposeProjects();
			if (!projects.Any())
			{
				ErrorHandling.LogError("There are no running projects.");
				return;
			}

			new ParallelTaskManager<string, TaskState>(projects, GetRow, (project, progress) => Execute(project, progress, options), HandleProgress, new[] { 0, 5 })
				.Execute();
		}

		private IEnumerable<string> GetRow(string project) =>
			new[] { project, TaskState.Pending.ToCell().Text };

		public void Execute(string project, IProgress<TaskState> progress, DownOptions options)
		{
			var results = DockerCompose.Down(project, options.PassthroughArguments, options.RemoveOrphans, false);
			progress.Report((results.ExitCode == 0).ToTaskState());
		}

		private void HandleProgress(TaskState taskState, TableRow row)
		{
			row.SetCell(taskState.ToCell(), 1);
		}
	}
}

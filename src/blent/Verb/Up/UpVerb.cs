using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blent.Verb.Up
{
	public class UpVerb : Verb<UpOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-up_options]";

		public override void Execute(UpOptions options)
		{
			var projects = options.Projects.Any() ? options.Projects : ProjectDirectory.GetProjects().Except(Docker.GetComposeProjects());
			if (!projects.Any())
			{
				ErrorHandling.LogError("There are no stopped projects.");
				return;
			}

			new ParallelTaskManager<string, TaskState>(projects, GetRow, (project, progress) => Execute(project, progress, options), HandleProgress, new[] { 0, 5 })
				.Execute();
		}

		private IEnumerable<string> GetRow(string project) =>
			new[] { project, TaskState.Pending.ToCell().Text };

		public void Execute(string project, IProgress<TaskState> progress, UpOptions options)
		{
			var results = DockerCompose.Up(project, options.PassthroughArguments, !options.Attach, options.ForceRecreate, false);
			progress.Report((results.ExitCode == 0).ToTaskState());
		}

		private void HandleProgress(TaskState taskState, TableRow row)
		{
			row.SetCell(taskState.ToCell(), 1);
		}
	}
}

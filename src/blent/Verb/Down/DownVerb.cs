using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blent.Verb.Down
{
	public class DownVerb : Verb<DownOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-down_options]";

		private Table _table;

		public override void Execute(DownOptions options)
		{
			var projects = options.Projects.Any() ? options.Projects : Docker.GetComposeProjects();
			if (!projects.Any())
			{
				ErrorHandling.LogError("There are no running projects.");
				return;
			}

			var progress = new Progress<(int, bool)>(HandleProgress);
			_table = new Table(projects.Select(p => new[] { p, "..." }), new[] { 0, 5 });
			var tableRenderer = new TableRenderer(_table, Output.Out);

			Parallel.ForEach(projects, (project, state, index) => {
				Run((int)index, project, options.PassthroughArguments, options.RemoveOrphans, progress);
			});

			tableRenderer.StopUpdating();
		}

		public void Run(int row, string project, string arguments, bool removeOrphans, IProgress<(int, bool)> progress)
		{
			var results = DockerCompose.Down(project, arguments, removeOrphans, false);
			progress.Report((row, results.ExitCode == 0));
		}

		private void HandleProgress((int row, bool success) progress)
		{
			var cell = progress.success ? new TableCell("done", Color.Success) : new TableCell("error", Color.Danger);
			_table.SetCell(cell, progress.row, 1);
		}
	}
}

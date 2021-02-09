using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blent.Verb.Up
{
	public class UpVerb : Verb<UpOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-up_options]";

		private Table _table;

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

			var progress = new Progress<(int, bool)>(HandleProgress);
			_table = new Table(projects.Select(p => new[] { p, "..." }), new[] { 0, 5 });
			var tableRenderer = new TableRenderer(_table, Output.Out);

			Parallel.ForEach(projects, (project, state, index) => {
				Run((int)index, project, options, progress);
			});

			tableRenderer.StopUpdating();
		}

		public void Run(int row, string project, UpOptions options, IProgress<(int, bool)> progress)
		{
			var results = DockerCompose.Up(project, options.PassthroughArguments, !options.Attach, options.ForceRecreate, false);
			progress.Report((row, results.ExitCode == 0));
		}

		private void HandleProgress((int row, bool success) progress)
		{
			var cell = progress.success ? new TableCell("done", Color.Success) : new TableCell("error", Color.Danger);
			_table.SetCell(cell, progress.row, 1);
		}
	}
}

using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blent.Verb.Update
{
	public class UpdateVerb : Verb<UpdateOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [options] [--] [docker-compose-up_options]";

		private Table _table;

		public override void Execute(UpdateOptions options)
		{
			var projects = options.Projects.Any() ? options.Projects : Docker.GetComposeProjects();
			if (!projects.Any())
			{
				ErrorHandling.LogError("There are no running projects.");
				return;
			}

			var progress = new Progress<(int, bool, bool?)>(HandleProgress);
			_table = new Table(new[] { "Project", "Pull", "Restart" }, projects.Select(p => new[] { p, "...", "" }), new[] { 0, 5, 0 });
			var tableRenderer = new TableRenderer(_table, Output.Out);

			Parallel.ForEach(projects, (project, state, index) =>
			{
				Run((int)index, project, progress);
			});

			tableRenderer.StopUpdating();

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

		public void Run(int row, string project, IProgress<(int, bool, bool?)> progress)
		{
			DockerCompose.Pull(project);
			progress.Report((row, true, null));

			DockerCompose.Up(project, printOutput: false);
			progress.Report((row, true, true));
		}

		private void HandleProgress((int row, bool pullSuccess, bool? upSuccess) progress)
		{
			SetCell(progress.pullSuccess, progress.row, 1);
			SetCell(progress.upSuccess, progress.row, 2);
		}

		private void SetCell(bool? success, int row, int column)
		{
			if (success.HasValue)
			{
				var cell = success.Value ? new TableCell("done", Color.Success) : new TableCell("error", Color.Danger);
				_table.SetCell(cell, row, column);
			}
			else
			{
				_table.SetCell("...", row, column);
			}
		}
	}
}

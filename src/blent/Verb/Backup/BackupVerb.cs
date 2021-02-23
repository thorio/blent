using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blent.Verb.Backup
{
	public class BackupVerb : Verb<BackupOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => "[PROJECT...] [options]";

		public override void Execute(BackupOptions options)
		{
			var projects = (options.Projects.Any() ? options.Projects : ProjectDirectory.GetProjects()).ToList();
			if (!projects.Any())
			{
				ErrorHandling.LogError("There are no projects to back up.");
				return;
			}

			Directory.CreateDirectory(options.BackupDirectory);

			new ParallelTaskManager<string, TaskState>(projects, GetRow, (project, progress) => Execute(project, progress, options), HandleProgress,
				new[] { 0, 5 })
				.Execute();

			TrimBackups(projects, options.BackupDirectory, options.NumberOfBackups);
		}

		private IEnumerable<string> GetRow(string project) =>
			new[] { project, TaskState.Pending.ToCell().Text };

		public void Execute(string project, IProgress<TaskState> progress, BackupOptions options)
		{
			var projectDirectory = ProjectDirectory.GetProjectDirectory(project);
			var archivePath = Path.Combine(options.BackupDirectory, $"{project}_{DateTime.Now.ToUnixTimestampMillis()}.zip");

			try
			{
				ZipFile.CreateFromDirectory(projectDirectory, archivePath, options.CompressionLevel, false);
				progress.Report(TaskState.Success);
			}
			catch
			{
				progress.Report(TaskState.Failure);
				// TODO show exception message
			}
		}

		private void HandleProgress(TaskState taskState, TableRow row)
		{
			row.SetCell(taskState.ToCell(), 1);
		}

		private void TrimBackups(IList<string> projects, string backupDirectory, int numberToKeep)
		{
			Output.Out.Write("\nDeleting old backups ... ", Color.Info);

			var projectsToDelete = Directory.GetFiles(backupDirectory)
				.Select(p => new FileInfo(p))
				.GroupBy(f => Regex.Match(f.Name, @"(.*)_").Groups[1].Value) // group by project
				.Where(g => projects.Contains(g.Key)) // filter out projects we don't care about
				.SelectMany(g => g
					.OrderByDescending(f => f.CreationTime)
					.Skip(numberToKeep)) // select the oldest files over the limit
				.ToList();

			foreach (var project in projectsToDelete)
			{
				project.Delete();
			}

			Output.Out.WriteLine($"{projectsToDelete.Count} backups deleted.", Color.Success);
		}
	}
}

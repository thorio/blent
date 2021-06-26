using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;
using System.Collections.Generic;
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

		private bool _encounteredError = false;

		public override void Execute(BackupOptions options, ILogger logger)
		{
			var projects = (options.Projects.Any() ? options.Projects : ProjectDirectory.GetProjects()).ToList();
			projects = projects.Distinct().ToList();

			if (!projects.Any())
			{
				var message = "Selection did not match any valid targets for backup";
				logger.Error(message, new { selection = options.Projects });
				ErrorPrinter.Error(message);
				return;
			}

			Directory.CreateDirectory(options.BackupDirectory);

			logger.Trace("writing archives in parallel", new { project_count = projects.Count(), projects = string.Join(", ", projects) });

			new ParallelTaskManager<string, TaskState>(projects, GetRow, (project, progress) => Execute(logger, project, progress, options), HandleProgress,
				new[] { 0, 5 })
				.Execute();

			if (_encounteredError)
			{
				ErrorPrinter.Warn("Some backups were unsuccessful, skipping backup deletion.");
				logger.Warn("some backups were unsuccessful, skipping backup deletion");
				return;
			}

			PruneBackups(logger, projects, options.BackupDirectory, options.NumberOfBackups);
		}

		private IEnumerable<string> GetRow(string project) =>
			new[] { project, TaskState.Pending.ToCell().Text };

		public void Execute(ILogger logger, string project, IProgress<TaskState> progress, BackupOptions options)
		{
			logger.Trace("writing archive", new { project });

			var projectDirectory = ProjectDirectory.GetProjectDirectory(project);
			var archivePath = Path.Combine(options.BackupDirectory, $"{project}_{DateTime.Now.ToUnixTimestampMillis()}.zip");

			try
			{
				ZipFile.CreateFromDirectory(projectDirectory, archivePath, options.CompressionLevel, false);
				logger.Info("writing archive succeeded", new { project, source = projectDirectory, target = archivePath });

				progress.Report(TaskState.Success);
			}
			catch (Exception ex)
			{
				_encounteredError = true;
				progress.Report(TaskState.Failure);

				ErrorPrinter.HandledException($"Writing archive of project [{project}] failed", ex);
				logger.Error("writing archive failed", ex, new { project, source = projectDirectory, target = archivePath });
			}
		}

		private void HandleProgress(TaskState taskState, TableRow row)
		{
			row.SetCell(taskState.ToCell(), 1);
		}

		private void PruneBackups(ILogger logger, IList<string> projects, string backupDirectory, int numberToKeep)
		{
			Output.Fancy.Write("Deleting old backups ... ", Color.Info);
			logger.Trace("determining archives to be deleted");

			var filesToDelete = Directory.GetFiles(backupDirectory)
				.Select(p => new FileInfo(p))
				.GroupBy(f => Regex.Match(f.Name, @"(.*)_").Groups[1].Value) // group by project
				.Where(g => projects.Contains(g.Key)) // filter out projects we don't care about
				.SelectMany(g => g
					.OrderByDescending(f => f.CreationTime)
					.Skip(numberToKeep)) // select the oldest files over the limit
				.ToList();

			var failCount = 0;

			foreach (var file in filesToDelete)
			{
				try
				{
					logger.Trace("deleting archive", new { target = file.FullName });
					file.Delete();
				}
				catch (Exception ex)
				{
					failCount++;

					if (failCount == 1)
					{
						Output.Fancy.WriteLine();
					}

					ErrorPrinter.HandledException($"Deleting archive [{file.FullName}] failed", ex);
					logger.Error("archive deletion failed", new { target = file.FullName });
				}
			}

			Output.Fancy.WriteLine($"{filesToDelete.Count - failCount} backups deleted.", Color.Success);
			if (failCount > 0)
			{
				ErrorPrinter.Error($"Deletion of {failCount} backups failed.");
			}

			logger.Info("archives deleted", new { total_count = filesToDelete.Count, failed_count = failCount });
		}
	}
}

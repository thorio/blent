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
		public override string Usage => "[STACK...] [options]";

		private bool _encounteredError = false;

		public override void Execute(BackupOptions options, ILogger logger)
		{
			var stacks = (options.Stacks.Any() ? options.Stacks : AppDirectory.GetStacks()).ToList();
			stacks = stacks.Distinct().ToList();

			if (!stacks.Any())
			{
				var message = "Selection did not match any valid targets for backup";
				logger.Error(message, new { selection = options.Stacks });
				ErrorPrinter.Error(message);
				return;
			}

			Directory.CreateDirectory(options.BackupDirectory);

			logger.Trace("writing archives in parallel", new { stack_count = stacks.Count(), stacks = string.Join(",", stacks) });

			new ParallelTaskManager<string, TaskState>(stacks, GetRow, (stack, progress) => Execute(logger, stack, progress, options), HandleProgress,
				new[] { 0, 5 })
				.Execute();

			if (_encounteredError)
			{
				ErrorPrinter.Warn("Some backups were unsuccessful, skipping backup deletion.");
				logger.Warn("some backups were unsuccessful, skipping backup deletion");
				return;
			}

			PruneBackups(logger, stacks, options.BackupDirectory, options.NumberOfBackups);
		}

		private IEnumerable<string> GetRow(string stack) =>
			new[] { stack, TaskState.Pending.ToCell().Text };

		public void Execute(ILogger logger, string stack, IProgress<TaskState> progress, BackupOptions options)
		{
			logger.Trace("writing archive", new { stack });

			var stackDirectory = AppDirectory.GetStackDirectory(stack);
			var archivePath = Path.Combine(options.BackupDirectory, $"{stack}_{DateTime.Now.ToUnixTimestampMillis()}.zip");

			try
			{
				ZipFile.CreateFromDirectory(stackDirectory, archivePath, options.CompressionLevel, false);
				logger.Info("writing archive succeeded", new { stack, source = stackDirectory, target = archivePath });

				progress.Report(TaskState.Success);
			}
			catch (Exception ex)
			{
				_encounteredError = true;
				progress.Report(TaskState.Failure);

				ErrorPrinter.HandledException($"Writing archive of stack {stack} failed", ex);
				logger.Error("writing archive failed", ex, new { stack, source = stackDirectory, target = archivePath });
			}
		}

		private void HandleProgress(TaskState taskState, TableRow row)
		{
			row.SetCell(taskState.ToCell(), 1);
		}

		private void PruneBackups(ILogger logger, IList<string> stacks, string backupDirectory, int numberToKeep)
		{
			Output.Fancy.Write("Deleting old backups ... ", Color.Info);
			logger.Trace("determining archives to be deleted");

			var filesToDelete = Directory.GetFiles(backupDirectory)
				.Select(p => new FileInfo(p))
				.GroupBy(f => Regex.Match(f.Name, @"(.*)_").Groups[1].Value) // group by stack
				.Where(g => stacks.Contains(g.Key)) // filter out stacks we don't care about
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

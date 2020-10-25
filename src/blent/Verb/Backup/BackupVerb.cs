using Blent.Interop;
using Blent.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blent.Verb.Backup
{
	public class BackupVerb : Verb<BackupOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => "[PROJECT...] [options]";

		public override void Execute(BackupOptions options)
		{
			var projects = GetProjects(options.Projects);

			Directory.CreateDirectory(options.BackupDirectory);

			Output.Out.WriteLine("Writing backups ... ", Color.Info);
			WriteBackups(projects, options.BackupDirectory, options.CompressionLevel);

			Output.Out.Write("\nDeleting old backups ... ", Color.Info);
			var backupsTrimmed = TrimBackups(projects, options.BackupDirectory, options.NumberOfBackups);
			Output.Out.WriteLine($"{backupsTrimmed} backups deleted.", Color.Success);
		}

		private IList<string> GetProjects(IEnumerable<string> projects)
		{
			var projectList = projects.ToList();
			if (!projectList.Any())
			{
				projectList = ProjectDirectory.GetProjects().ToList();
			}

			if (!projectList.Any())
			{
				ErrorHandling.LogFatalAndQuit("There are no projects to back up.");
			}

			return projectList;
		}

		private void WriteBackups(IList<string> projects, string backupDirectory, CompressionLevel compressionLevel)
		{
			var maxLength = projects.Max(p => p.Length);
			Parallel.ForEach(projects, (project) =>
			{
				var watch = new Stopwatch();
				watch.Start();
				var projectDirectory = ProjectDirectory.GetProjectDirectory(project);
				var archivePath = Path.Combine(backupDirectory, $"{project}_{DateTime.Now.ToUnixTimestampMillis()}.zip");

				ZipFile.CreateFromDirectory(projectDirectory, archivePath, compressionLevel, false);
				Output.Out.WriteLine($"archived {project.PadRight(maxLength)} in {watch.Elapsed.TotalSeconds:0.##}s", Color.Default);
			});
		}

		private int TrimBackups(IList<string> projects, string backupDirectory, int numberToKeep)
		{
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

			return projectsToDelete.Count;
		}
	}
}

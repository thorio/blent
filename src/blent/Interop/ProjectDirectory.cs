using Blent.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blent.Interop
{
	public static class ProjectDirectory
	{
		public static IEnumerable<string> GetProjects()
		{
			return Directory.GetDirectories(Settings.GetAppDirectory())
				.Select(d => Path.GetFileName(d))
				.Where(p => p.First() != '.');
		}

		public static bool ProjectExists(string project) =>
			GetProjects().Contains(project);

		public static string GetProjectDirectory(string projectName)
		{
			return Path.Combine(Settings.GetAppDirectory(), projectName);
		}
	}
}

using Blent.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blent.Interop
{
	public static class AppDirectory
	{
		public static IEnumerable<string> GetStacks()
		{
			return Directory.GetDirectories(Settings.GetAppDirectory())
				.Select(d => Path.GetFileName(d))
				.Where(p => p.First() != '.');
		}

		public static bool StackExists(string stack) =>
			GetStacks().Contains(stack);

		public static string GetStackDirectory(string stack)
		{
			return Path.Combine(Settings.GetAppDirectory(), stack);
		}
	}
}

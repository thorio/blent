using Blent.Configuration;
using Blent.Utility;
using System.Collections.Generic;
using System.Diagnostics;

namespace Blent.Interop
{
	public static class DockerCompose
	{
		private const string Command = "docker-compose";

		public static void Run(string project, string arguments, bool printOutput)
		{
			if (!ProjectDirectory.ProjectExists(project))
			{
				ErrorHandling.LogError($"Project '{project}' not found.");
				return;
			}

			var process = Process.Start(new ProcessStartInfo()
			{
				FileName = Command,
				WorkingDirectory = $"{Settings.AppDirectory}/{project}",
				Arguments = arguments,
				RedirectStandardOutput = !printOutput,
				RedirectStandardError = !printOutput,
			});
			process.WaitForExit();
		}

		public static void Run(IEnumerable<string> projects, string arguments, bool printOutput)
		{
			foreach (var project in projects)
			{
				Run(project, arguments, printOutput);
			}
		}

		public static void Up(IEnumerable<string> projects, string arguments) =>
			Run(projects, "up -d " + arguments, true);

		public static void ReUp(IEnumerable<string> projects, string arguments) =>
			Up(projects, "--force-recreate " + arguments);

		public static void Down(IEnumerable<string> projects, string arguments) =>
			Run(projects, "down " + arguments, true);

		public static void Logs(string project, IEnumerable<string> services, bool follow = false, bool showTimestamps = false)
		{
			var arguments = $"logs {string.Join(' ', services)}";
			if (follow) arguments += " -f";
			if (showTimestamps) arguments += " -t";
			Run(project, arguments, true);
		}
	}
}

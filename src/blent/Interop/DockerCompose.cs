using Blent.Utility;
using System.Collections.Generic;

namespace Blent.Interop
{
	public static class DockerCompose
	{
		private const string Command = "docker-compose";

		public static ProcessResults RunIn(string workingDirectory, string arguments, bool printOutput) =>
			Process.Run(Command, arguments, workingDirectory, printOutput, true);

		public static ProcessResults Run(string project, string arguments, bool printOutput)
		{
			if (!ProjectDirectory.ProjectExists(project))
			{
				ErrorHandling.LogError($"Project '{project}' not found.");
				return null;
			}

			return RunIn(ProjectDirectory.GetProjectDirectory(project), arguments, printOutput);
		}

		public static void Run(IEnumerable<string> projects, string arguments, bool printOutput)
		{
			foreach (var project in projects)
			{
				Run(project, arguments, printOutput);
			}
		}

		public static void Up(IEnumerable<string> projects, string extraArguments = "", bool detached = true, bool forceRecreate = false)
		{
			var arguments = $"up ";
			if (detached) arguments += "-d ";
			if (forceRecreate) arguments += "--force-recreate ";
			Run(projects, arguments + extraArguments, true);
		}

		public static void Down(IEnumerable<string> projects, string extraArguments = "", bool removeOrphans = false)
		{
			var arguments = $"down ";
			if (removeOrphans) arguments += "--remove-orphans ";
			Run(projects, arguments + extraArguments, true);
		}

		public static void Logs(string project, IEnumerable<string> services, bool follow = false, bool showTimestamps = false)
		{
			var arguments = $"logs {string.Join(' ', services)}";
			if (follow) arguments += " -f";
			if (showTimestamps) arguments += " -t";
			Run(project, arguments, true);
		}

		public static void Pull(IEnumerable<string> projects) =>
			Run(projects, "pull", true);

		public static void Exec(string project, string service, string command, int serviceIndex = 1, string extraArguments = "")
		{
			var arguments = $"exec --index={serviceIndex} {extraArguments} {service} {command}";
			Run(project, arguments, true);
		}
	}
}

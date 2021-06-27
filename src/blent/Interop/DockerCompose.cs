using Blent.Utility;
using System.Collections.Generic;

namespace Blent.Interop
{
	public static class DockerCompose
	{
		private const string Command = "docker-compose";

		public static ProcessResult RunIn(string workingDirectory, string arguments, bool printOutput) =>
			Process.Run(Command, arguments, workingDirectory, printOutput, printOutput);

		public static ProcessResult Run(string stack, string arguments, bool printOutput)
		{
			if (!AppDirectory.StackExists(stack))
			{
				Output.Logger.Error("stack not found", new { stack });
				ErrorPrinter.Error($"Stack '{stack}' not found.");
				return new ProcessResult(1);
			}

			return RunIn(AppDirectory.GetStackDirectory(stack), arguments, printOutput);
		}

		public static void Run(IEnumerable<string> stacks, string arguments, bool printOutput)
		{
			foreach (var stack in stacks)
			{
				Run(stack, arguments, printOutput);
			}
		}

		public static ProcessResult Up(string stack, string extraArguments = "", bool detached = true, bool forceRecreate = false, bool printOutput = true)
		{
			var arguments = $"up ";
			if (detached) arguments += "-d ";
			if (forceRecreate) arguments += "--force-recreate ";
			return Run(stack, arguments + extraArguments, printOutput);
		}

		public static ProcessResult Down(string stack, string extraArguments = "", bool removeOrphans = false, bool printOutput = true)
		{
			var arguments = $"down ";
			if (removeOrphans) arguments += "--remove-orphans ";
			return Run(stack, arguments + extraArguments, printOutput);
		}

		public static void Logs(string stack, IEnumerable<string> services, bool follow = false, bool showTimestamps = false)
		{
			var arguments = "logs";
			if (follow) arguments += " -f";
			if (showTimestamps) arguments += " -t";
			arguments += $" {string.Join(' ', services)}";
			Run(stack, arguments, true);
		}

		public static ProcessResult Pull(string stack) =>
			Run(stack, "pull", false);

		public static void Exec(string stack, string service, string command, int serviceIndex = 1, string extraArguments = "")
		{
			var arguments = $"exec --index={serviceIndex} {extraArguments} {service} {command}";
			Run(stack, arguments, true);
		}
	}
}

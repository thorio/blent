using Blent.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Blent.Interop
{
	public static class Docker
	{
		public const string Command = "docker";
		public const string ComDockerComposeProject = "com.docker.compose.project";

		public static string Run(string arguments)
		{
			var process = Process.Start(new ProcessStartInfo()
			{
				FileName = Command,
				Arguments = arguments,
				RedirectStandardOutput = true,
			});
			process.WaitForExit();
			return process.StandardOutput.ReadToEnd();
		}

		public static string Inspect(IEnumerable<string> containers, string format = null, string additionalArguments = "")
		{
			var arguments = $"inspect {string.Join(' ', containers)} {additionalArguments}";

			if (!string.IsNullOrEmpty(format))
			{
				arguments += $" --format=\"{format.EscapeDoubleQuotes()}\"";
			}

			return Run(arguments);
		}

		public static IEnumerable<string> GetContainers(string filter = null, string additionalArguments = "")
		{
			var arguments = $"ps -q {additionalArguments}";

			if (!string.IsNullOrEmpty(filter))
			{
				arguments += $" --filter \"{filter.EscapeDoubleQuotes()}\"";
			}

			return Run(arguments).AsList(Environment.NewLine);
		}

		public static IEnumerable<string> GetComposeProjects()
		{
			var containers = GetContainers($"label={ComDockerComposeProject}");
			if (!containers.Any()) return new string[0];

			return Inspect(containers, $"{{{{index .Config.Labels \"{ComDockerComposeProject}\"}}}}")
				.AsList(Environment.NewLine)
				.Distinct();
		}

	}
}

using Blent.Utility;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Blent.Interop
{
	public static class Process
	{
		public static ProcessResult Run(string command, string arguments, string workingDirectory, bool printOutput, bool printErrors)
		{
			try
			{
				Output.Logger.Debug("starting child process", new { command, arguments, workingDirectory });
				PerformanceTesting.Checkpoint($"Begin Process {command} {arguments.Split(' ').First()}");

				var process = System.Diagnostics.Process.Start(new ProcessStartInfo()
				{
					FileName = command,
					WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
					Arguments = arguments,
					RedirectStandardOutput = !printOutput,
					RedirectStandardError = !printErrors,
				});
				process.WaitForExit();

				var output = printOutput ? null : process.StandardOutput.ReadToEnd();
				var error = printErrors ? null : process.StandardError.ReadToEnd();

				Output.Logger.Trace("end child process", new { command, arguments, workingDirectory, process_stdout = output, process_stderr = error });

				PerformanceTesting.Checkpoint($"End Process {command}");
				return new ProcessResult(process.ExitCode, output, error);
			}
			catch (Win32Exception ex)
			{
				Output.Logger.Fatal("missing dependency", new { dependency = command });
				throw new FatalException($"{command} is not available.", ex);
			}
		}
	}
}

using Blent.Utility;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Blent.Interop
{
	public static class Process
	{
		public static ProcessResults Run(string command, string arguments, string workingDirectory, bool printOutput, bool printErrors)
		{
			try
			{
				Output.Logger.Debug("starting process", new { command, arguments, workingDirectory });
				PerformanceTesting.Checkpoint($"Begin Process [{command} {arguments.Split(' ').First()}]");

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

				PerformanceTesting.Checkpoint($"End Process {command}");
				return new ProcessResults(process.ExitCode, output, error);
			}
			catch (Win32Exception)
			{
				Output.Logger.Fatal("missing dependency", new { dependency = command });
				ErrorPrinter.FatalAndQuit($"{command} is not available.");
				throw;
			}
		}
	}
}

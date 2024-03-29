using Blent.Configuration;
using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blent.Verb.Down
{
	public class DownVerb : Verb<DownOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[STACK...] [options] [--] [docker-compose-down_options]";

		public override void Execute(DownOptions options, ILogger logger)
		{
			var stacks = options.Stacks.Any() ? options.Stacks : Docker.GetComposeStacks();
			if (!stacks.Any())
			{
				logger.Error("no running stacks found");
				ErrorPrinter.Error("There are no running stacks.");
				return;
			}

			logger.Trace("stopping stacks in parallel", new { stack_count = stacks.Count(), stacks = string.Join(",", stacks) });

			new ParallelTaskManager<string, TaskState>(stacks, GetRow, (stack, progress) => Execute(stack, progress, options, logger), HandleProgress, new[] { 0, 5 })
				.Execute(Settings.UserConfig.Parallelism);
		}

		private IEnumerable<string> GetRow(string stack) =>
			new[] { stack, TaskState.Pending.ToCell().Text };

		public void Execute(string stack, IProgress<TaskState> progress, DownOptions options, ILogger logger)
		{
			logger.Trace("stopping stack", new { stack });

			var result = DockerCompose.Down(stack, options.PassthroughArguments, options.RemoveOrphans, false);
			if (result.ExitCode == 0)
			{
				logger.Info("stopped stack", new { stack });
				progress.Report(TaskState.Success);
			}
			else
			{
				logger.Error("stopping stack failed", new { stack });
				logger.Debug("stopping stack failed", new { stack, compose_stdout = result.Output, compose_stderr = result.Error });

				ErrorPrinter.SubProcessError($"Stopping stack {stack} failed.", "docker-compose down", result);
				progress.Report(TaskState.Failure);
			}
		}

		private void HandleProgress(TaskState taskState, TableRow row)
		{
			row.SetCell(taskState.ToCell(), 1);
		}
	}
}

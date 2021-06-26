using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blent.Verb.Up
{
	public class UpVerb : Verb<UpOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[STACK...] [options] [--] [docker-compose-up_options]";

		public override void Execute(UpOptions options, ILogger logger)
		{
			var stacks = options.Stacks.Any() ? options.Stacks : AppDirectory.GetStacks().Except(Docker.GetComposeStacks());
			if (!stacks.Any())
			{
				logger.Error("no stopped stacks found");
				ErrorPrinter.Error("There are no stopped stacks.");
				return;
			}

			logger.Trace("starting stacks in parallel", new { stack_count = stacks.Count(), stacks = string.Join(", ", stacks) });

			new ParallelTaskManager<string, TaskState>(stacks, GetRow, (stack, progress) => Execute(stack, progress, options, logger), HandleProgress, new[] { 0, 5 })
				.Execute();
		}

		private IEnumerable<string> GetRow(string stack) =>
			new[] { stack, TaskState.Pending.ToCell().Text };

		public void Execute(string stack, IProgress<TaskState> progress, UpOptions options, ILogger logger)
		{
			logger.Trace("starting stack", new { stack });

			var results = DockerCompose.Up(stack, options.PassthroughArguments, !options.Attach, options.ForceRecreate, false);
			if (results.ExitCode == 0)
			{
				logger.Info("started stack", new { stack });
				progress.Report(TaskState.Success);
			}
			else
			{
				logger.Error("starting stack failed", new { stack });
				logger.Debug("starting stack failed", new { stack, compose_stdout = results.Output, compose_stderr = results.Error });
				progress.Report(TaskState.Failure);
			}
		}

		private void HandleProgress(TaskState taskState, TableRow row)
		{
			row.SetCell(taskState.ToCell(), 1);
		}
	}
}

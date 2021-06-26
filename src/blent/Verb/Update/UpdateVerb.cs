using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blent.Verb.Update
{
	public class UpdateVerb : Verb<UpdateOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[STACK...] [options] [--] [docker-compose-up_options]";

		public override void Execute(UpdateOptions options, ILogger logger)
		{
			var stacks = options.Stack.Any() ? options.Stack : Docker.GetComposeStacks();
			if (!stacks.Any())
			{
				logger.Error("no running stacks found");
				ErrorPrinter.Error("There are no running stacks.");
				return;
			}

			logger.Trace("updating stacks in parallel", new { stack_count = stacks.Count(), stacks = string.Join(", ", stacks) });

			new ParallelTaskManager<string, (TaskState, TaskState)>(stacks, GetRow, (stack, progress) => Execute(stack, progress, options, logger), HandleProgress,
				new[] { 0, 5, 0 }, new[] { "Stack", "Pull", "Restart" })
				.Execute();

			if (options.RemoveDanglingImages)
			{
				RemoveDanglingImages(logger);
			}
		}

		private IEnumerable<string> GetRow(string stack) =>
			new[] { stack, TaskState.Pending.ToCell().Text, "" };

		public void Execute(string stack, IProgress<(TaskState, TaskState)> progress, UpdateOptions options, ILogger logger)
		{
			logger.Trace("pulling stack", new { stack });

			var pullResults = DockerCompose.Pull(stack);
			var pullSuccess = pullResults.ExitCode == 0;
			progress.Report((pullSuccess.ToTaskState(), TaskState.Pending));

			if (!pullSuccess)
			{
				logger.Error("pulling stack failed", new { stack });
				logger.Debug("pulling stack failed", new { stack, compose_stdout = pullResults.Output, compose_stderr = pullResults.Error });
				return;
			}

			logger.Trace("recreating stack", new { stack });

			var upResults = DockerCompose.Up(stack, printOutput: false);
			var upSuccess = upResults.ExitCode == 0;
			progress.Report((TaskState.Success, upSuccess.ToTaskState()));

			if (!upSuccess)
			{
				logger.Error("recreating stack failed", new { stack });
				logger.Debug("recreating stack failed", new { stack, compose_stdout = upResults.Output, compose_stderr = upResults.Error });
				return;
			}

			logger.Info("recreated stack", new { stack });
		}

		private void HandleProgress((TaskState pullState, TaskState upState) report, TableRow row)
		{
			row.SetCell(report.pullState.ToCell(), 1);
			row.SetCell(report.upState.ToCell(), 2);
		}

		private void RemoveDanglingImages(ILogger logger)
		{
			logger.Trace("removing dangling images");
			Output.Fancy.WriteLine("\nRemoving dangling images ...", Color.Info);

			var images = Docker.GetImages("dangling=true");
			if (images.Any())
			{
				Docker.RemoveImages(images);
				Output.Fancy.WriteLine($"Removed {images.Count()} images.");
				logger.Info("dangling images removed", new { count = images.Count() });
			}
			else
			{
				logger.Debug("no dangling images found");
				Output.Fancy.WriteLine("No dangling images found.");
			}
		}
	}
}

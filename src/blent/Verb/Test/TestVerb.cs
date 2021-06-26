#if DEBUG
using Blent.Utility;
using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;
using System.Collections.Generic;

namespace Blent.Verb.Test
{
	public class TestVerb : Verb<TestOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => null;

		public override void Execute(TestOptions options, ILogger logger)
		{

			new ParallelTaskManager<string, TaskState>(new[] { "testa", "testb" }, GetRow, DoStuff, HandleProgress,
				new[] { 0, 5 })
				.Execute();
		}

		private void DoStuff(string p, IProgress<TaskState> progress)
		{
			Output.Error.WriteLine("testing123");
			progress.Report(TaskState.Success);
		}

		private IEnumerable<string> GetRow(string project)
		{
			return new[] { project, TaskState.Pending.ToCell().Text };
		}

		private void HandleProgress(TaskState taskState, TableRow row)
		{
			row.SetCell(taskState.ToCell(), 1);
		}
	}
}
#endif

using System;

namespace Blent.Utility.Drawing
{
	public static class TaskStateExtensions
	{
		public static TableCell ToCell(this TaskState taskState) {
			return taskState switch
			{
				TaskState.Pending => new TableCell("...", Color.Default),
				TaskState.Success => new TableCell("done", Color.Success),
				TaskState.Failure => new TableCell("error", Color.Danger),
				TaskState.NotStarted => new TableCell("", Color.Default),
				_ => throw new ArgumentException("Unhandled enum value")
			};
		}
	}
}

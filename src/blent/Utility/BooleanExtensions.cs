namespace Blent.Utility
{
	public static class BooleanExtensions
	{
		public static TaskState ToTaskState(this bool success) =>
			success ? TaskState.Success : TaskState.Failure;

		public static TaskState ToSuccessiveTaskState(this bool success) =>
			success ? TaskState.Pending : TaskState.NotStarted;
	}
}

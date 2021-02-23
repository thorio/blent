namespace Blent.Utility
{
	public static class BooleanExtensions
	{
		public static TaskState ToTaskState(this bool success) =>
			success ? TaskState.Success : TaskState.Failure;

	}
}

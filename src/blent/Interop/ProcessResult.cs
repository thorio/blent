namespace Blent.Interop
{
	public class ProcessResult
	{
		public ProcessResult(int exitCode, string output = null, string error = null)
		{
			ExitCode = exitCode;
			Output = output;
			Error = error;
		}

		public int ExitCode { get; }
		public string Output { get; }
		public string Error { get; }
	}
}

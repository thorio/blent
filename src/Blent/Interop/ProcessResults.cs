namespace Blent.Interop
{
	public class ProcessResults
	{
		public ProcessResults(int exitCode, string output)
		{
			ExitCode = exitCode;
			Output = output;
		}

		public int ExitCode { get; set; }	
		public string Output { get; set; }	
	}
}

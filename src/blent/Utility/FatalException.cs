using System;

namespace Blent.Utility
{
	public class FatalException : Exception
	{
		public FatalException(string message, Exception innerException = null, int exitCode = 1) : base(message, innerException)
		{
			ExitCode = exitCode;
		}

		public int ExitCode { get; set; }
	}
}

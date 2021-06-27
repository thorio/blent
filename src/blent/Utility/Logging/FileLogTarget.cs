using System;
using System.IO;

namespace Blent.Utility.Logging
{
	public class FileLogTarget : TextWriterLogTarget, IDisposable
	{
		public FileLogTarget(string path) : base(OpenFile(path)) { }

		public void Dispose()
		{
			_writer.Flush();
			_writer.Dispose();
		}

		private static StreamWriter OpenFile(string path)
		{
			try
			{
				return File.AppendText(path);
			}
			catch(Exception ex)
			{
				// logging anything here wouldn't work, the logger still has a null target at this point.
				throw new FatalException($"Opening Log File [{path}] failed.", ex);
			}
		}
	}
}

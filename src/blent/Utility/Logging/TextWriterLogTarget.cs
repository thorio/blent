using System.IO;

namespace Blent.Utility.Logging
{
	public class TextWriterLogTarget : ILogTarget
	{
		protected readonly TextWriter _writer;

		/// <summary>
		/// Write lifetime is controlled by the caller.
		/// </summary>
		public TextWriterLogTarget(TextWriter writer)
		{
			_writer = writer;
		}

		public void WriteLine(string value) => _writer.WriteLine(value);
	}
}

using System.IO;

namespace Blent.Utility.Logging
{
	public class TextWriterLogTarget : ILogTarget
	{
		private TextWriter _writer;

		public TextWriterLogTarget(TextWriter writer)
		{
			_writer = writer;
		}

		public void WriteLine(string value) => _writer.WriteLine(value);
	}
}

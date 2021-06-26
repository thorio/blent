using System.IO;

namespace Blent.Utility.Drawing
{
	public class TextWriterOutputTarget : IOutputTarget
	{
		private TextWriter _textWriter;

		public TextWriterOutputTarget(TextWriter textWriter) =>
			_textWriter = textWriter;

		public void Write(string value) =>
			_textWriter.Write(value);
	}
}

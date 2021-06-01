using System.IO;

namespace Blent.Utility.Drawing
{
	public class OutputWriter
	{
		private readonly TextWriter _textWriter;

		public OutputWriter(TextWriter textWriter)
		{
			_textWriter = textWriter;
			Colors = new ColorHelper();
		}

		public ColorHelper Colors { get; set; }
		public bool Enabled { get; set; }

		public void Write(string text, Color color = Color.Default)
		{
			if (!Enabled) return;
			_textWriter.Write(Colors.Get(color) + text.NormalizeLineEndings());
		}

		public void WriteLine()
		{
			if (!Enabled) return;
			_textWriter.WriteLine();
		}

		public void WriteLine(string text, Color color = Color.Default)
		{
			if (!Enabled) return;
			_textWriter.WriteLine(Colors.Get(color) + text.NormalizeLineEndings());
		}

		public void ResetStyling()
		{
			if (!Enabled) return;
			_textWriter.Write(Colors.GetReset());
		}
	}
}

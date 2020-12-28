using System;
using System.IO;

namespace Blent.Utility.Drawing
{
	public class Output
	{
		public static Output Out { get; }
		public static Output Error { get; }

		private readonly TextWriter _textWriter;

		static Output()
		{
			Out = new Output(Console.Out);
			Error = new Output(Console.Error);
		}

		public Output(TextWriter textWriter)
		{
			_textWriter = textWriter;
			Colors = new ColorHelper();
		}

		public ColorHelper Colors { get; set; }

		public void Write(string text, Color color = Color.Default)
		{
			_textWriter.Write(Colors.Get(color) + text.NormalizeLineEndings());
		}

		public void WriteLine()
		{
			_textWriter.WriteLine();
		}

		public void WriteLine(string text, Color color = Color.Default)
		{
			_textWriter.WriteLine(Colors.Get(color) + text.NormalizeLineEndings());
		}
	}
}

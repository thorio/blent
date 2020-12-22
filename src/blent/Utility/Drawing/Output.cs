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
		}

		public void Write(string text, Color color = Color.Default)
		{
			var previousColor = SetColor(color);
			_textWriter.Write(text.NormalizeLineEndings());
			Console.ForegroundColor = previousColor;
		}

		public void WriteLine()
		{
			_textWriter.WriteLine();
		}


		public void WriteLine(string text, Color color = Color.Default)
		{
			var previousColor = SetColor(color);
			_textWriter.WriteLine(text.NormalizeLineEndings());
			Console.ForegroundColor = previousColor;
		}

		private ConsoleColor SetColor(Color color)
		{
			var originalColor = Console.ForegroundColor;
			ConsoleColor? newColor = color switch
			{
				Color.Default => null,
				Color.Muted => ConsoleColor.DarkGray,
				Color.Primary => ConsoleColor.Gray,
				Color.Success => ConsoleColor.DarkGreen,
				Color.Info => ConsoleColor.DarkCyan,
				Color.Warning => ConsoleColor.Yellow,
				Color.Danger => ConsoleColor.Red,
				_ => null,
			};

			Console.ForegroundColor = newColor ?? originalColor;
			return originalColor;
		}
	}
}

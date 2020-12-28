using System.Collections.Generic;

namespace Blent.Utility.Drawing
{
	public class ColorHelper
	{
		private const string Esc = "\u001b";
		private readonly IDictionary<Color, string> _colorTable;

		public ColorHelper(IDictionary<Color, string> colorTable = null)
		{
			_colorTable = colorTable ?? GetDefaultColorTable();
		}

		public string Get(Color color)
		{
			return _colorTable[color];
		}

		private static Dictionary<Color, string> GetDefaultColorTable()
		{
			return new Dictionary<Color, string>()
			{
				{Color.Default, $"{Esc}[39m"}, // default
				{Color.Muted, $"{Esc}[90m"}, // dark gray
				{Color.Primary, $"{Esc}[37m"}, // light gray
				{Color.Success, $"{Esc}[32m"}, // dark green
				{Color.Info, $"{Esc}[36m"}, // dark cyan
				{Color.Warning, $"{Esc}[33m"}, // dark yellow
				{Color.Danger, $"{Esc}[31m"}, // dark red
			};
		}
	}
}

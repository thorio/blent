namespace Blent.Utility.Drawing
{
	public class TableCell : IReadOnlyTableCell
	{
		public TableCell(string content, Color color = Color.Default)
		{
			Text = content;
			Color = color;
		}

		public TableCell(IReadOnlyTableCell cell)
		{
			Text = cell.Text;
			Color = cell.Color;
		}

		public string Text { get; set; }
		public Color Color { get; set; }
	}
}

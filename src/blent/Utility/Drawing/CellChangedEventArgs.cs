namespace Blent.Utility.Drawing
{
	public class CellChangedEventArgs
	{
		public IReadOnlyTableCell Cell { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }
	}
}

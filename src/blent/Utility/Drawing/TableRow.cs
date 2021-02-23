using System.Collections.Generic;

namespace Blent.Utility.Drawing
{
	public class TableRow
	{
		private readonly Table _table;
		private readonly int _row;

		public TableRow(Table table, int row)
		{
			_table = table;
			_row = row;
		}

		public IReadOnlyTableCell GetCell(int column) =>
			_table.GetCell(_row, column);

		public IEnumerable<IReadOnlyTableCell> GetCells() =>
			_table.GetRowCells(_row);

		public void SetCell(IReadOnlyTableCell cell, int column) =>
			_table.SetCell(cell, _row, column);
	}
}

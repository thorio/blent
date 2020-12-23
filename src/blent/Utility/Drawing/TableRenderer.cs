using Blent.Utility.Drawing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blent.Utility.Drawing
{
	public class TableRenderer
	{
		private const int ColumnDividerWidth = 2;
		private readonly Table _table;
		private bool _finished;

		public TableRenderer(Table table)
		{
			_table = table;
			_table.CellChanged += OnCellChanged;
			DrawTable();
		}

		private void OnCellChanged(object sender, CellChangedEventArgs e)
		{
			if (!_finished)
			{
				Console.Write(DrawCellAt(e.Cell, e.Row, e.Column));
			}
		}

		public void StopUpdating()
		{
			Console.Write("\n");
			_finished = true;
		}

		public void DrawTable()
		{
			var builder = new StringBuilder();
			var tableWidth = _table.GetColumnWidths().Sum(w => w + ColumnDividerWidth);

			builder.AppendLine(DrawRow(_table.GetHeaderRow()));
			builder.AppendLine(new string('-', tableWidth));

			var rows = _table.GetData().Select(r => DrawRow(r));
			builder.AppendJoin('\n', rows);
			builder.Append("\r");

			Console.Write(builder.ToString());
		}

		private string DrawRow(IEnumerable<IReadOnlyTableCell> cells)
		{
			var columnWidths = _table.GetColumnWidths();
			var cellValues = cells.Select((cell, i) => DrawCell(cell, columnWidths[i]));

			return string.Concat(cellValues);
		}

		private string DrawCellAt(IReadOnlyTableCell cell, int row, int column)
		{
			var columnWidths = _table.GetColumnWidths();
			var shiftY = _table.RowCount - (row + 1);
			var shiftX = columnWidths.Take(column).Sum(w => w + ColumnDividerWidth);
			return DrawAt(DrawCell(cell, columnWidths[column]), shiftX, shiftY);
		}

		private string DrawCell(IReadOnlyTableCell cell, int columnWidth) =>
			cell.Text.PadRight(columnWidth + ColumnDividerWidth);

		/// <summary>
		/// Moves the cursor to a relative position and prints the string, then moves the cursor back.
		/// </summary>
		private string DrawAt(string str, int relativeX, int relativeY) =>
			$"\u001b[{relativeY}A\u001b[{relativeX}C{str}\u001b[{relativeY}B\r";
	}
}

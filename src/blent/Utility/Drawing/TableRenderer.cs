using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blent.Utility.Drawing
{
	public class TableRenderer
	{
		private const int ColumnDividerWidth = 3;
		private readonly Table _table;
		private readonly Output _output;
		private bool _finished;

		public TableRenderer(Table table, Output output)
		{
			_table = table;
			_output = output;
			_table.CellChanged += OnCellChanged;
			DrawTable();
		}

		private void OnCellChanged(object sender, CellChangedEventArgs e)
		{
			if (!_finished)
			{
				_output.Write(DrawCellAt(e.Cell, e.Row, e.Column));
			}
		}

		public void StopUpdating()
		{
			_output.WriteLine();
			_finished = true;
		}

		public void DrawTable()
		{
			var builder = new StringBuilder();

			if (_table.ShowHeader)
			{
				var tableWidth = _table.GetColumnWidths().Sum(w => w + ColumnDividerWidth);
				builder.AppendLine(DrawRow(_table.GetHeaderRow()));
				builder.AppendLine(new string('-', tableWidth));
			}

			var rows = _table.GetData().Select(r => DrawRow(r));
			builder.AppendJoin('\n', rows);
			builder.Append("\r");

			_output.Write(builder.ToString());
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
			var relativeY = _table.RowCount - (row + 1);
			var absoluteX = columnWidths.Take(column).Sum(w => w + ColumnDividerWidth);
			return DrawAt(DrawCell(cell, columnWidths[column]), absoluteX, relativeY);
		}

		private string DrawCell(IReadOnlyTableCell cell, int columnWidth)
		{
			var content = cell.Text;
			if (content.Length > columnWidth)
			{
				content = content.Substring(0, columnWidth);
			}

			return _output.Colors.Get(cell.Color) + content.PadRight(columnWidth + ColumnDividerWidth);
		}

		/// <summary>
		/// Moves the cursor to a relative position and prints the string, then moves the cursor back.
		/// </summary>
		private string DrawAt(string str, int absoluteX, int relativeY)
		{
			var up = relativeY > 0 ? $"\u001b[{relativeY}A" : "";
			var left = absoluteX > 0 ? $"\u001b[{absoluteX}C" : "";
			var down = relativeY > 0 ? $"\u001b[{relativeY}B" : "";
			return $"{up}{left}{str}{down}\r";
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Blent.Utility.Drawing
{
	public class Table
	{
		#region Fields
		private readonly IList<TableCell> _headerCells;
		private readonly IList<int> _columnWidths;
		private readonly IList<IList<TableCell>> _rows;
		#endregion

		#region Constructors
		public Table(IEnumerable<string> columnNames, IEnumerable<IEnumerable<string>> data, IEnumerable<int> columnWidths = null)
		{
			_rows = data.Select(StringsToCellList).ToList();
			_headerCells = StringsToCellList(columnNames);
			_columnWidths = columnWidths?.ToList() ?? new List<int>(new int[_headerCells.Count]);

			ComputeOptimalColumnWidths();
			ShowHeader = true;
		}

		public Table(IEnumerable<IEnumerable<string>> data, IEnumerable<int> columnWidths = null) : this(data.First().Select(d => ""), data, columnWidths)
		{
			ShowHeader = false;
		}
		#endregion

		#region Events
		public event EventHandler<CellChangedEventArgs> CellChanged;
		#endregion

		#region Properties
		public int RowCount => _rows.Count;
		public bool ShowHeader { get; set; }
		#endregion

		#region Public
		public IReadOnlyList<IReadOnlyList<IReadOnlyTableCell>> GetData() =>
			_rows.Select(r => GetReadOnlyRow(r)).ToArray();

		public IReadOnlyList<IReadOnlyTableCell> GetRow(int index) =>
			GetReadOnlyRow(_rows[index]);

		public IReadOnlyList<IReadOnlyTableCell> GetHeaderRow() =>
			GetReadOnlyRow(_headerCells);

		public IReadOnlyTableCell GetCell(int row, int column) =>
			_rows[row][column];

		public IReadOnlyList<int> GetColumnWidths() =>
			_columnWidths.ToArray();

		public void SetCell(string text, int row, int column)
		{
			_rows[row][column].Text = text;
			CellChanged(this, new CellChangedEventArgs() { Cell = _rows[row][column], Row = row, Column = column });
		}

		public void SetCell(IReadOnlyTableCell cell, int row, int column)
		{
			_rows[row][column] = new TableCell(cell);
			CellChanged(this, new CellChangedEventArgs() { Cell = cell, Row = row, Column = column });
		}
		#endregion

		#region Private
		private IList<TableCell> StringsToCellList(IEnumerable<string> strings) =>
			strings.Select(v => new TableCell(v)).ToList();

		private IReadOnlyList<IReadOnlyTableCell> GetReadOnlyRow(IList<TableCell> cells) =>
			cells.Cast<IReadOnlyTableCell>().ToArray();

		/// <summary>
		/// Sets the column widths to fit the contents. <br />
		/// Columns that already have a width (!= 0) are ignored.
		/// </summary>
		private void ComputeOptimalColumnWidths()
		{
			for (var i = 0; i < _headerCells.Count; i++)
			{
				if (_columnWidths[i] != 0) continue;
				_columnWidths[i] = _rows.Select(r => r[i])
					.Append(_headerCells[i])
					.Max(c => c.Text.Length);
			}
		}
		#endregion
	}
}

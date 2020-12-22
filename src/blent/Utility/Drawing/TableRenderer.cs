using Blent.Utility.Drawing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blent.Utility.Drawing
{
	public class TableRenderer
	{
		private const int ColumnDividerWidth = 2;
		private Table _table;
		private Output _output;

		public TableRenderer(Table table, Output output)
		{
			_table = table;
			_output = output;
		}

		public void Draw()
		{
			Console.Beep();
			_table.RecomputeLayout();
			DrawHeader();
			foreach (var row in _table.Rows)
			{
				DrawRow(row);
			}
		}

		private void DrawHeader()
		{
			var values = _table.Columns
				.Select(k => new KeyValuePair<int, string>(k.Key, k.Value.Name));
			DrawRow(values);

			var rowLength = _table.Columns.Values.Sum(c => c.Width + ColumnDividerWidth);
			_output.WriteLine(new string('-', rowLength));
		}

		private void DrawRow(TableRow row)
		{
			DrawRow(row.Values);
		}

		private void DrawRow(IEnumerable<KeyValuePair<int, string>> values)
		{
			var cells = values.Select((keyValuePair, i) =>
				{
					var column = _table.Columns[i];
					return keyValuePair.Value.PadRight(column.Width + ColumnDividerWidth);
				});
			_output.WriteLine(string.Concat(cells));
		}
	}
}

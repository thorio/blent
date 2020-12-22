using System.Collections.Generic;
using System.Linq;

namespace Blent.Utility.Drawing.Models
{
	public class Table
	{
		public Table(IEnumerable<string> columnHeaders, IEnumerable<IEnumerable<string>> data)
		{
			Columns = columnHeaders
				.Select((h, id) => new TableColumn(id, h))
				.ToDictionary(c => c.Id);

			Rows = data.Select(r => new TableRow(r)).ToList();
		}

		public IList<TableRow> Rows { get; set; }
		public IDictionary<int, TableColumn> Columns { get; set; }

		public void RecomputeLayout()
		{
			foreach (var column in Columns.Values)
			{
				var columnValues = Rows.Select(r => r.Values[column.Id]);
				column.CalculateWidth(columnValues);
			}
		}
	}
}

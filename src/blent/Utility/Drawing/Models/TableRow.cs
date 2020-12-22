using System.Collections.Generic;
using System.Linq;

namespace Blent.Utility.Drawing.Models
{
	public class TableRow
	{
		public TableRow(IEnumerable<string> values)
		{
			Values = values
				.Select((v, id) => (v, id))
				.ToDictionary(t => t.id, t => t.v);
		}

		public IDictionary<int, string> Values { get; set; }
	}
}

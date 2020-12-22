using System.Collections.Generic;
using System.Linq;

namespace Blent.Utility.Drawing.Models
{
	public class TableColumn
	{
		public TableColumn(int id, string name)
		{
			Id = id;
			Name = name;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public int Width { get; set; }

		public void CalculateWidth(IEnumerable<string> values)
		{
			Width = values
				.Append(Name)
				.Max(v => v.Length);
		}
	}
}

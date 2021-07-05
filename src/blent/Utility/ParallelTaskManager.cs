using Blent.Utility.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blent.Utility
{
	public class ParallelTaskManager<TParameter, TReport>
	{
		private readonly Table _table;
		private readonly IEnumerable<TParameter> _parameters;
		private readonly Action<TParameter, IProgress<TReport>> _executor;
		private readonly Action<TReport, TableRow> _progressHandler;

		public ParallelTaskManager(IEnumerable<TParameter> parameters, Func<TParameter, IEnumerable<string>> rowInitializer, Action<TParameter, IProgress<TReport>> executor,
			Action<TReport, TableRow> progressHandler, IEnumerable<int> columnWidths = null, IEnumerable<string> columnHeaders = null)
		{
			_table = new Table(parameters.Select(rowInitializer), columnWidths, columnHeaders);

			_parameters = parameters;
			_executor = executor;
			_progressHandler = progressHandler;
		}

		public void Execute()
		{
			var tableRenderer = new TableRenderer(_table, Output.Fancy);

			Parallel.ForEach(_parameters, (parameter, state, index) =>
			{
				var progress = new CustomProgress<TReport>((report) => _progressHandler(report, _table.GetRow((int)index)));

				_executor(parameter, progress);
			});

			tableRenderer.Dispose();
		}
	}
}

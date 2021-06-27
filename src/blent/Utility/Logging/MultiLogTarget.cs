using System.Collections.Generic;
using System.Linq;

namespace Blent.Utility.Logging
{
	/// <summary>
	/// Writes the incoming logs to many downstream targets.
	/// </summary>
	public class MultiLogTarget : ILogTarget
	{
		private List<ILogTarget> _targets;

		public MultiLogTarget(params ILogTarget[] targets)
		{
			_targets = targets.ToList();
		}

		public int TargetCount => _targets.Count;

		public void AddTarget(ILogTarget target)
		{
			_targets.Add(target);
		}

		public void WriteLine(string value) =>
			_targets.ForEach(t => t.WriteLine(value));
	}
}

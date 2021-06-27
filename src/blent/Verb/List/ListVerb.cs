using Blent.Interop;
using Blent.Utility;
using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;
using System.Linq;

namespace Blent.Verb.List
{
	public class ListVerb : Verb<ListOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => null;

		public override void Execute(ListOptions options, ILogger logger)
		{
			var stacks = AppDirectory.GetStacks().OrderBy(s => s).ToArray();
			var runningStacks = Docker.GetComposeStacks().ToArray();

			foreach (var stack in stacks)
			{
				var color = runningStacks.Contains(stack) ? Color.Success : Color.Danger;
				Output.Fancy.WriteLine(stack, color);
			}

			logger.Info("stack list", new { stack_count = stacks.Length, stacks = string.Join(",", stacks), running_stacks = string.Join(",", runningStacks) });
		}
	}
}

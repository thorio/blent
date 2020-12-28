using Blent.Utility.Drawing;
using System;
using System.Threading;

namespace Blent.Verb.Test
{
	public class TestVerb : Verb<TestOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => null;

		public override void Execute(TestOptions options)
		{
			var table = new Table(new[] { "name", "pull", "recreate" },
				new[] {
					new[] {"authelia", "done", "..."},
					new[] {"traefik", "55%", ""},
					new[] {"home", "done", "done"},
				},
				new[] {0, 10, 10}
			);

			var renderer = new TableRenderer(table, Output.Out);

			Thread.Sleep(1000);
			table.SetCell("80%", 1, 1);
			Thread.Sleep(1000);
			table.SetCell(new TableCell("done", Color.Success), 1, 1);
			table.SetCell("done", 1, 2);

			renderer.StopUpdating();
		}
	}
}

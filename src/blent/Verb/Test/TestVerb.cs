using Blent.Utility.Drawing;
using Blent.Utility.Drawing.Models;
using System.Threading;

namespace Blent.Verb.Test
{
	public class TestVerb : Verb<TestOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => null;

		public override void Execute(TestOptions options)
		{
			var table = new Table(new[] { "name", "pull", "recreate" }, new[] {
				new[] {"authelia", "done", "..."},
				new[] {"traefik", "55%", ""},
				new[] {"home", "done", "done"},
			});

			var renderer = new TableRenderer(table, Output.Out);
			renderer.Draw();
		}
	}
}

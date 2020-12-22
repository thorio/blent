using Blent.Interop;
using Blent.Utility.Drawing;
using System;
using System.Linq;

namespace Blent.Verb.List
{
	public class ListVerb : Verb<ListOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => null;

		public override void Execute(ListOptions options)
		{
			var projects = ProjectDirectory.GetProjects().OrderBy(s => s);
			var runningProjects = Docker.GetComposeProjects();

			foreach (var project in projects)
			{
				var color = runningProjects.Contains(project) ? Color.Success : Color.Danger;
				Output.Out.WriteLine(project, color);
			}
		}
	}
}

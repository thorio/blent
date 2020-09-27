using Blent.Interop;
using Blent.Utility;
using System;
using System.Linq;

namespace Blent.Verb.List
{
	public class ListVerb : Base.Verb<ListOptions>
	{
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

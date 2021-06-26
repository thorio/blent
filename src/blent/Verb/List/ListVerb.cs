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
			var projects = ProjectDirectory.GetProjects().OrderBy(s => s).ToArray();
			var runningProjects = Docker.GetComposeProjects().ToArray();

			foreach (var project in projects)
			{
				var color = runningProjects.Contains(project) ? Color.Success : Color.Danger;
				Output.Fancy.WriteLine(project, color);
			}

			logger.Info("project list", new { project_count = projects.Length, projects = string.Join(", ", projects), running_projects = string.Join(", ", runningProjects) });
		}
	}
}

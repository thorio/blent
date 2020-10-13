using Blent.Configuration;
using Blent.Interop;
using Blent.ProjectRepository;
using Blent.Properties;
using Blent.Utility;
using System.IO;

namespace Blent.Verb.CreateProject
{
	public class CreateProjectVerb : Verb<CreateProjectOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => "PROJECT [ID]";

		public override void Execute(CreateProjectOptions options)
		{
			//// TODO check if project already exists locally or in index

			//var id = options.Id ?? $"default/{options.Name}";

			//var repo = RepositoryProvider.GetDefaultRepository();
			//using var index = repo.OpenIndex();
			//var projectPath = Path.Combine(Settings.AppDirectory, options.Name);

			//Output.Out.WriteLine("Creating worktree ...", Color.Info);
			//Git.AddWorktree(repo.GetBasePath(), projectPath, Constants.RepositoryIndexBranchName, true);

			//Output.Out.WriteLine("Creating branch ...", Color.Info);
			//Git.CreateOrphanBranch(projectPath, id);

			//Output.Out.WriteLine("Adding files ...", Color.Info);
			//File.WriteAllText(Path.Combine(projectPath, "docker-compose.yml"), Resources.default_compose_yml.ToString().NormalizeLineEndings());

			//// TODO update index

			//Output.Out.WriteLine("Pushing ...", Color.Info);
			//Git.AddCommitPush(projectPath, "CREATE PROJECT", $"origin {id}");
			//// TODO push index
		}
	}
}

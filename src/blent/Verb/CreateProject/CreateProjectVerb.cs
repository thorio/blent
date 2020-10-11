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
			var id = options.Id ?? $"default/{options.Name}";

			var repo = new Repository();
			using var index = repo.OpenIndex();
			var projectPath = Path.Combine(Settings.AppDirectory, options.Name);

			Output.Out.WriteLine("Creating worktree ...", Color.Info);
			Git.AddWorktree(repo.GetBasePath(), projectPath, Constants.RepositoryIndexBranchName, true);

			// TODO check if folder already exists locally, as creating the orphan branch deletes all files in the folder.
			Output.Out.WriteLine("Creating branch ...", Color.Info);
			Git.CreateOrphanBranch(projectPath, id);

			Output.Out.WriteLine("Adding default files ...", Color.Info);
			File.WriteAllText(Path.Combine(projectPath, "docker-compose.yml"), Resources.default_compose_yml.ToString().NormalizeLineEndings());

			// TODO update index

			Output.Out.WriteLine("Pushing ...", Color.Info);
			Git.AddCommitPush(projectPath, "CREATE PROJECT", $"origin {id}");
			// TODO push index
		}
	}
}

using Blent.Utility;
using Blent.Utility.Logging;

namespace Blent.Verb.Version
{
	public class VersionVerb : Verb<VersionOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => "[options]";

		public override void Execute(VersionOptions options, ILogger logger)
		{
			var version = AssemblyInfo.GetVersion();
			var name = AssemblyInfo.GetName();
			var commit = AssemblyInfo.GetShortCommitHash();

			if (options.Short)
			{
				Output.Basic.WriteLine(version);
				return;
			}

			Output.Basic.WriteLine($"{name} version {version}, build {commit}");
			Output.Basic.WriteLine($"Free Software licensed under {Constants.License}");
			Output.Basic.WriteLine($"Website: {Constants.ProjectUrl}");
			Output.Basic.WriteLine($"Source Code: {Constants.RepositoryUrl}");
			Output.Basic.WriteLine($"Issues: {Constants.IssuesUrl}");

			logger.Info(null, new {
				name,
				version,
				commit = Constants.CommitHash,
				license = Constants.License,
				project_url = Constants.ProjectUrl,
				repository_url = Constants.RepositoryUrl,
				issues_url = Constants.IssuesUrl,
			});
		}
	}
}

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
				Output.Out.WriteLine(version);
				return;
			}

			Output.Out.WriteLine($"{name} version {version}, build {commit}");
			Output.Out.WriteLine($"Free Software licensed under {Constants.License}");
			Output.Out.WriteLine($"Website: {Constants.ProjectUrl}");
			Output.Out.WriteLine($"Source Code: {Constants.RepositoryUrl}");
			Output.Out.WriteLine($"Issues: {Constants.IssuesUrl}");

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

using Blent.Utility;
using System.Reflection;

namespace Blent.Verb.Version
{
	public class VersionVerb : Verb<VersionOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => "[options]";

		public override void Execute(VersionOptions options)
		{
			var assembly = Assembly.GetAssembly(typeof(VersionVerb));
			var version = ((AssemblyInformationalVersionAttribute)assembly
				.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)[0])
				.InformationalVersion;
			var name = assembly.GetName().Name;

			if (options.Short)
			{
				Output.Out.WriteLine(version);
				return;
			}

			Output.Out.WriteLine($"{name} version {version}, build {Constants.CommitHash.Substring(0, 8)}");
			Output.Out.WriteLine($"Free Software licensed under {Constants.License}");
			Output.Out.WriteLine($"Website: {Constants.ProjectUrl}");
			Output.Out.WriteLine($"Source Code: {Constants.RepositoryUrl}");
			Output.Out.WriteLine($"Issues: {Constants.IssuesUrl}");
		}
	}
}

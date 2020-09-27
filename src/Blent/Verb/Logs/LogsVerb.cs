using Blent.Interop;

namespace Blent.Verb.Logs
{
	public class LogsVerb : Base.Verb<LogsOptions>
	{
		public override void Execute(LogsOptions options)
		{
			DockerCompose.Logs(options.Project, options.Services, options.Follow, options.ShowTimestamps);
		}
	}
}

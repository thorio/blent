using Blent.Interop;

namespace Blent.Verb.Logs
{
	public class LogsVerb : Verb<LogsOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "PROJECT [SERVICE...] [options]";

		public override void Execute(LogsOptions options)
		{
			DockerCompose.Logs(options.Project, options.Services, options.Follow, options.ShowTimestamps);
		}
	}
}

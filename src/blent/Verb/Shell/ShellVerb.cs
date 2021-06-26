using Blent.Interop;
using Blent.Utility.Logging;

namespace Blent.Verb.Shell
{
	public class ShellVerb : Verb<ShellOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "PROJECT SERVICE [--] [docker-compose-exec_options]";

		public override void Execute(ShellOptions options, ILogger logger)
		{
			logger.Warn("logging shell output is not supported");

			DockerCompose.Exec(options.Project, options.Service, options.Shell, extraArguments: options.PassthroughArguments);
		}
	}
}

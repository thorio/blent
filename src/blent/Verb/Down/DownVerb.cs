namespace Blent.Verb.Down
{
	public class DownVerb : Verb<DownOptions>
	{
		public override bool RequiresDocker => true;
		public override string Usage => "[PROJECT...] [-- docker-compose_down-args]";

		public override void Execute(DownOptions options)
		{
			Interop.DockerCompose.Down(options.Values, string.Join(" ", options.Rest));
		}
	}
}

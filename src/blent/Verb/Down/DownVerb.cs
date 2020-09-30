namespace Blent.Verb.Down
{
	public class DownVerb : Verb<DownOptions>
	{
		public override bool RequiresDocker => true;

		public override void Execute(DownOptions options)
		{
			Interop.DockerCompose.Down(options.Values, string.Join(" ", options.Rest));
		}
	}
}

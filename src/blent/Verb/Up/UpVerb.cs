namespace Blent.Verb.Up
{
	public class UpVerb : Verb<UpOptions>
	{
		public override bool RequiresDocker => true;

		public override void Execute(UpOptions options)
		{
			Interop.DockerCompose.Up(options.Values, string.Join(" ", options.Rest));
		}
	}
}

namespace Blent.Verb.Down
{
	public class DownVerb : Base.Verb<DownOptions>
	{
		public override void Execute(DownOptions options)
		{
			Interop.DockerCompose.Down(options.Values, string.Join(" ", options.Rest));
		}
	}
}

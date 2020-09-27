namespace Blent.Verb.Up
{
	public class UpVerb : Base.Verb<UpOptions>
	{
		public override void Execute(UpOptions options)
		{
			Interop.DockerCompose.Up(options.Values, string.Join(" ", options.Rest));
		}
	}
}

namespace Blent.Verb.Test
{
	public class TestVerb : Verb<TestOptions>
	{
		public override bool RequiresDocker => true;

		public override void Execute(TestOptions options)
		{

		}
	}
}

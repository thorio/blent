#if DEBUG
namespace Blent.Verb.Test
{
	public class TestVerb : Verb<TestOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => null;

		public override void Execute(TestOptions options)
		{
			new Down.DownVerb().Execute(new Down.DownOptions() { Projects = new[] { "doesntexist", "doesntexisteither" } });
		}
	}
}
#endif

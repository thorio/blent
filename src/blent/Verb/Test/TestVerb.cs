#if DEBUG
using Blent.Configuration;
using Blent.Utility.Logging;

namespace Blent.Verb.Test
{
	public class TestVerb : Verb<TestOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => null;

		public override void Execute(TestOptions options, ILogger logger)
		{
			var a = Environment.CurrentPlatform;
		}
	}
}
#endif

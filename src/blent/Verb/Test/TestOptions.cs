#if DEBUG
using CommandLine;
using System.Collections.Generic;

namespace Blent.Verb.Test
{
	[Verb("test", Hidden = true)]
	public class TestOptions : Options
	{
		[Value(0)]
		public IEnumerable<string> Values { get; set; }
	}
}
#endif

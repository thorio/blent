using System;

namespace Blent.Verb.Base
{
	public abstract class Verb<TOptions> : IVerb where TOptions : IOptions
	{
		public Verb() { }

		public void Execute(IOptions options) =>
			Execute((TOptions)options);

		public Type GetOptionsType() =>
			typeof(TOptions);

		public abstract void Execute(TOptions options);
	}
}

using System;

namespace Blent.Verb
{
	public interface IVerb
	{
		bool RequiresDocker { get; }

		void Execute(IOptions options);
		Type GetOptionsType();
	}
}

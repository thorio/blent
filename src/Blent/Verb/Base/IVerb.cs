using System;

namespace Blent.Verb.Base
{
	public interface IVerb
	{
		void Execute(IOptions options);
		Type GetOptionsType();
	}
}

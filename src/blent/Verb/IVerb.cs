using System;

namespace Blent.Verb
{
	public interface IVerb
	{
		bool RequiresDocker { get; }
		string Usage { get; }

		void Execute(IOptions options);
		Type GetOptionsType();
		string GetVerbName();
	}
}

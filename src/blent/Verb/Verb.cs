using Blent.Utility.Logging;
using CommandLine;
using System;

namespace Blent.Verb
{
	public abstract class Verb<TOptions> : IVerb where TOptions : IOptions
	{
		public void Execute(IOptions options, ILogger logger) =>
			Execute((TOptions)options, logger);

		public Type GetOptionsType() =>
			typeof(TOptions);

		public string GetVerbName()
		{
			var attribute = (VerbAttribute)Attribute.GetCustomAttribute(GetOptionsType(), typeof(VerbAttribute));
			return attribute.Name;
		}

		public abstract void Execute(TOptions options, ILogger logger);

		public abstract bool RequiresDocker { get; }

		public abstract string Usage { get; }
	}
}

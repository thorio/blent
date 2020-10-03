using CommandLine;
using System;

namespace Blent.Verb
{
	public abstract class Verb<TOptions> : IVerb where TOptions : IOptions
	{
		public Verb() { }

		public void Execute(IOptions options) =>
			Execute((TOptions)options);

		public Type GetOptionsType() =>
			typeof(TOptions);

		public string GetVerbName()
		{
			var attribute = (VerbAttribute)Attribute.GetCustomAttribute(GetOptionsType(), typeof(VerbAttribute));
			return attribute.Name;
		}

		public abstract void Execute(TOptions options);

		/// <summary>
		/// Indicates whether docker and -compose availability should be checked before execution.
		/// </summary>
		public abstract bool RequiresDocker { get; }

		public abstract string Usage { get; }
	}
}

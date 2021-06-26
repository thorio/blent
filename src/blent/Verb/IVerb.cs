using Blent.Utility.Logging;
using System;

namespace Blent.Verb
{
	public interface IVerb
	{
		/// <summary>
		/// Indicates whether docker availability should be checked before execution.
		/// </summary>
		bool RequiresDocker { get; }

		/// <summary>
		/// Usage message. See <a href="https://en.wikipedia.org/wiki/Usage_message">Wikipedia article</a>. <br />
		/// e.g. "[STACK...] [-- docker-compose_down-args]"
		/// </summary>
		string Usage { get; }

		void Execute(IOptions options, ILogger logger);
		
		/// <summary>
		/// Must return the Options Type (which implements <see cref="IOptions"/>).
		/// </summary>
		Type GetOptionsType();

		/// <summary>
		///	Must return the Verb's name.
		/// </summary>
		string GetVerbName();
	}
}

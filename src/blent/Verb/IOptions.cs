using Blent.Utility;
using Blent.Utility.Logging;

namespace Blent.Verb
{
	public interface IOptions
	{
		/// <summary>
		/// All arguments after `--`.
		/// </summary>
		string PassthroughArguments { get; set; }

		/// <summary>
		/// See <see cref="Configuration.Settings.GetAppDirectory()"/>.
		/// </summary>
		string AppDirectory { get; set; }
		LogLevel LogLevel { get; set; }
		OutputMode OutputMode { get; set; }
		string LogFile { get; set; }
		int? Parallelism { get; set; }
	}
}

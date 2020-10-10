namespace Blent.Verb
{
	public interface IOptions
	{
		/// <summary>
		/// All arguments after `--`.
		/// </summary>
		string PassthroughArguments { get; set; }

		/// <summary>
		/// See <see cref="Configuration.Settings.AppDirectory"/>.
		/// </summary>
		string AppDirectory { get; set; }
	}
}

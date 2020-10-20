namespace Blent
{
	/// <summary>
	/// Contains global constants that are needed in more than one class.<br />
	/// Tags (ie. $$TAG_NAME$$) are replaced by the build server.
	/// </summary>
	public static class Constants
	{
		// metadata, build info
		public const string ProjectUrl = "https://github.com/$$APPVEYOR_REPO_NAME$$#readme";
		public const string RepositoryUrl = "https://github.com/$$APPVEYOR_REPO_NAME$$";
		public const string IssuesUrl = "https://github.com/$$APPVEYOR_REPO_NAME$$/issues";
		public const string License = "GPL-3.0";
		public const string CommitHash = "$$APPVEYOR_REPO_COMMIT$$";
	}
}

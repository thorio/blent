namespace Blent.ProjectRepository
{
	public static class RepositoryProvider
	{
		private const string DefaultRepositoryName = "default";

		public static Repository GetDefaultRepository()
		{
			return new Repository(DefaultRepositoryName);
		}

		public static Repository CreateDefaultRepository(string remoteUrl)
		{
			return Repository.Create(DefaultRepositoryName, remoteUrl);
		}
	}
}

using Blent.Configuration;
using Blent.Utility;
using System.IO;

namespace Blent.ProjectRepository
{
	public class Repository
	{
		private const string IndexFileName = "index.yml";
		private const string RepositoriesFolderName = "repositories";
		private const string DefaultRepositoryName = "default";

		private readonly string _repositoryName;

		public Repository()
		{
			_repositoryName = DefaultRepositoryName;
		}

		public Models.RepositoryIndex GetIndex() =>
			YamlFile<Models.RepositoryIndex>.Read(GetIndexFilePath());

		public YamlFile<Models.RepositoryIndex> OpenIndex() =>
			YamlFile<Models.RepositoryIndex>.Open(GetIndexFilePath());

		private string GetIndexFilePath() =>
			Path.Combine(GetBasePath(), IndexFileName);

		public string GetBasePath() =>
			Path.Combine(Settings.AppDirectory, Constants.AppDirectoryBlentFolder, RepositoriesFolderName, _repositoryName);
	}
}

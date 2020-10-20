using Blent.Configuration;
using Blent.Interop;
using Blent.ProjectRepository.Models;
using Blent.Utility;
using System.IO;

namespace Blent.ProjectRepository
{
	public class Repository
	{
		private const string IndexFileName = "index.yml";
		private const string RepositoriesFolderName = "repos";
		private const string IndexFileVersion = "unstable";

		private readonly string _repositoryName;

		public Repository(string repositoryName)
		{
			_repositoryName = repositoryName;
			Git = new Git(GetBasePath());
		}

		public Git Git { get; }

		public static Repository Create(string repositoryName, string remoteUrl)
		{
			var repo = new Repository(repositoryName);

			if (Directory.Exists(Path.Combine(repo.GetBasePath(), ".git")))
			{
				ErrorHandling.LogFatalAndQuit("Repository already exists.");
			}

			Directory.CreateDirectory(repo.GetBasePath());
			repo.Git.Init();
			repo.Git.AddRemote(remoteUrl);
			repo.Git.CreateBranch(Constants.RepositoryIndexBranchName);

			using (var indexFile = repo.OpenIndex())
			{
				indexFile.Write(new RepositoryIndex { IndexVersion = IndexFileVersion, Projects = new RepositoryIndexProject[0] });
			}

			repo.Git.CommitAll("CREATE INDEX");
			repo.Git.Push("origin HEAD");

			return repo;
		}

		public Project AddProject(string name, string id = null)
		{
			id ??= $"default/{name}";
			var project = new Project(this, name, id);
			Git.CreateOrphanBranch(id);
			Git.AddWorktree(project.GetPath(), id);
			return project;
		}

		public RepositoryIndex GetIndex() =>
			YamlFile<RepositoryIndex>.Read(GetIndexFilePath());

		public YamlFile<RepositoryIndex> OpenIndex() =>
			YamlFile<RepositoryIndex>.Open(GetIndexFilePath());

		private static string GetBasePath(string repositoryName) =>
			Path.Combine(Settings.AppDirectory, Constants.AppDirectoryBlentFolder, RepositoriesFolderName, repositoryName);

		private string GetIndexFilePath() =>
			Path.Combine(GetBasePath(), IndexFileName);

		private string GetBasePath() =>
			GetBasePath(_repositoryName);
	}
}

using Blent.Configuration;
using Blent.Interop;
using Blent.Utility;
using System.IO;

namespace Blent.ProjectRepository
{
	public class Project
	{
		public Project(Repository repository, string name, string id)
		{
			Repository = repository;
			Name = name;
			Id = id;
			Git = new Git(GetPath());
		}

		public Repository Repository { get; }
		public string Name { get; }
		public string Id { get; }
		public Git Git { get; }

		public void AddFile(string path, byte[] content) =>
			AddFile(path, content.ToString());

		public void AddFile(string path, string content) =>
			File.WriteAllText(Path.Combine(GetPath(), path), content.NormalizeLineEndings());

		public string GetPath() =>
			Path.Combine(Settings.AppDirectory, Name);
	}
}

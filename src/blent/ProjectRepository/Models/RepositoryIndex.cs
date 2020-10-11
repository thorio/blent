using System.Collections.Generic;

namespace Blent.ProjectRepository.Models
{
	public class RepositoryIndex
	{
		public string Version { get; set; }
		public string Name { get; set; }
		public IEnumerable<ProjectIndexEntry> Projects { get; set; }
	}
}

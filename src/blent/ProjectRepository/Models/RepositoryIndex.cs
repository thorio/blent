using System.Collections.Generic;

namespace Blent.ProjectRepository.Models
{
	public class RepositoryIndex
	{
		public string IndexVersion { get; set; }
		public IEnumerable<RepositoryIndexProject> Projects { get; set; }
	}
}

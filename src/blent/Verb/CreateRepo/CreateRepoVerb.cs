using Blent.ProjectRepository;

namespace Blent.Verb.CreateRepo
{
	public class CreateRepoVerb : Verb<CreateRepoOptions>
	{
		public override bool RequiresDocker => false;
		public override string Usage => "REMOTE";

		public override void Execute(CreateRepoOptions options)
		{
			RepositoryProvider.CreateDefaultRepository(options.RemoteUrl);
		}
	}
}

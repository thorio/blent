using Blent.Utility;

namespace Blent.Interop
{
	public class Git
	{
		public const string Command = "git";

		public readonly string _gitDir;

		public Git(string gitDir)
		{
			_gitDir = gitDir;
		}

		public ProcessResults Run(string arguments, bool printOutput = false, bool printErrors = true) =>
			Process.Run(Command, arguments, _gitDir, printOutput, printErrors);

		public ProcessResults AddWorktree(string path, string branch, bool force = false)
		{
			var arguments = $"worktree add {path} --checkout {branch} ";
			if (force) arguments += "-f ";
			return Run(arguments);
		}

		public ProcessResults CreateBranch(string branch) =>
			Run($"checkout -b {branch} -q");

		public ProcessResults Reset(string target, bool hard = false)
		{
			var arguments = $"reset {target} ";
			if (hard) arguments += "--hard ";
			return Run(arguments);
		}

		public void Init() =>
			Run("init");

		public ProcessResults AddRemote(string url, string name = "origin") =>
			Run($"remote add {name} {url}");

		public void CommitAll(string message)
		{
			Run("add .");
			Run($"commit -m \"{message.EscapeDoubleQuotes()}\"");
		}

		public ProcessResults Push(string setUpstream = null)
		{
			var arguments = "push -q ";
			if (!string.IsNullOrWhiteSpace(setUpstream)) arguments += $"-u {setUpstream} ";
			return Run(arguments);
		}

		public void CreateOrphanBranch(string branch)
		{
			Run($"checkout --orphan {branch} -q");
			Reset("", true);
		}
	}
}

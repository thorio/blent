using Blent.Utility;
using System;
using System.IO;

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

		public ProcessResults Run(string arguments, bool printOutput = false, bool printErrors = true)
		{
			Output.Error.WriteLine("git " + arguments, Color.Warning);
			return Process.Run(Command, arguments, _gitDir, printOutput, printErrors);
		}

		public Git AddWorktree(string path, string branch, bool force = false)
		{
			var arguments = $"worktree add {path} --checkout {branch} ";
			if (force) arguments += "-f ";
			var results = Run(arguments);
			return results.ExitCode == 0 ? new Git(path) : null;
		}

		public ProcessResults RemoveWorktree(string path) =>
			Run($"worktree remove {path}");

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

		public void CommitAll(string message, bool allowEmpty = false)
		{
			Run("add .");

			var arguments = $"commit -m \"{message.EscapeDoubleQuotes()}\" ";
			if (allowEmpty) arguments += "--allow-empty ";
			Run(arguments);
		}

		public ProcessResults Push(string setUpstream = null)
		{
			var arguments = "push -q ";
			if (!string.IsNullOrWhiteSpace(setUpstream)) arguments += $"-u {setUpstream} ";
			return Run(arguments);
		}

		public void CreateOrphanBranch(string branch)
		{
			var tmpdir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n"));

			var worktree = AddWorktree(tmpdir, "HEAD", true);
			worktree.Run($"checkout --orphan {branch} -q");
			worktree.Reset("", true);
			worktree.CommitAll("CREATE BRANCH", true);
			RemoveWorktree(tmpdir);
		}
	}
}

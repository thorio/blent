using Blent.Utility;
using System;

namespace Blent.Interop
{
	public static class Git
	{
		public const string Command = "git";

		public static ProcessResults Run(string arguments, string workingDirectory = null, bool printOutput = false, bool printErrors = true) =>
			Process.Run(Command, arguments, workingDirectory, printOutput, printErrors);

		public static ProcessResults AddWorktree(string repoPath, string path, string branch, bool force = false)
		{
			var arguments = $"worktree add {path} --checkout {branch} ";
			if (force) arguments += "-f ";
			return Run(arguments, repoPath);
		}

		public static ProcessResults Checkout(string repoPath, string branch, bool isOrphan = false, bool quiet = false)
		{
			var arguments = "checkout ";
			if (quiet) arguments += "-q ";
			if (isOrphan) arguments += "--orphan ";
			return Run(arguments + branch, repoPath);
		}

		public static ProcessResults Reset(string repoPath, string target, bool hard = false)
		{
			var arguments = $"reset {target} ";
			if (hard) arguments += "--hard ";
			return Run(arguments, repoPath);
		}

		public static void AddCommitPush(string repoPath, string message, string setUpstream = null, bool quiet = false)
		{
			Run("add .", repoPath);
			Run($"commit -m \"{message.EscapeDoubleQuotes()}\"", repoPath);

			var arguments = "push ";
			if (!string.IsNullOrWhiteSpace(setUpstream)) arguments += $"-u {setUpstream} ";
			if (quiet) arguments += "-q ";
			Run(arguments, repoPath);
		}

		public static void CreateOrphanBranch(string repoPath, string branch)
		{
			Checkout(repoPath, branch, true, true);
			Reset(repoPath, "", true);
		}
	}
}

using System.Reflection;

namespace Blent.Utility
{
	public static class AssemblyInfo
	{
		private static Assembly _assembly;

		private static Assembly GetAssembly() =>
			_assembly ??= Assembly.GetAssembly(typeof(AssemblyInfo));

		public static string GetVersion()
		{
			return ((AssemblyInformationalVersionAttribute)GetAssembly()
				.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)[0])
				.InformationalVersion;
		}

		public static string GetName() =>
			GetAssembly().GetName().Name;

		public static string GetShortCommitHash() =>
			Constants.CommitHash.Substring(0, 8);
	}
}

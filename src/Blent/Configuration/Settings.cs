using Microsoft.Extensions.Configuration;
using System.IO;

namespace Blent.Configuration
{
	public static class Settings
	{
		private static string _appDirectory;
		private static UserConfig _userConfig;

		public static string AppDirectory
		{
			get => _appDirectory ??= GetAppDirectory();
			set => _appDirectory = CheckAppDirectory(value);
		}

		public static UserConfig UserConfig => _userConfig ??= GetUserConfig();

		private static UserConfig GetUserConfig()
		{
			var defaultConfigFileProvider = new StringFilerProvider(Properties.Resources.default_user_yml);
			return new ConfigurationBuilder()
				.AddYamlFile(defaultConfigFileProvider, "default.user.yml", false, false)
				.AddYamlFile($"{Environment.UserHomeDirectory}/.config/blent/user.yml", true, false)
				.Build()
				.Get<UserConfig>();
		}

		private static string GetAppDirectory()
		{
			var appDirectory = UserConfig.AppDirectory.Replace("~", Environment.UserHomeDirectory);
			return CheckAppDirectory(Path.GetFullPath(appDirectory));
		}

		private static string CheckAppDirectory(string path)
		{
			return Directory.Exists(path)
				? path
				: Utility.ErrorHandling.LogFatalAndQuit<string>($"AppDirectory [{path}] does not exist.");
		}
	}
}

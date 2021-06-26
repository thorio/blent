using Blent.Configuration.Models;
using Blent.Utility;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Blent.Configuration
{
	public static class Settings
	{
		private static string _appDirectory;
		private static UserConfig _userConfig;

		public static string GetAppDirectory()
		{
			if (_appDirectory == null)
			{
				var appDirectory = GetUserConfig().AppDirectory.Replace("~", Environment.UserHomeDirectory);
				_appDirectory = CheckAppDirectory(Path.GetFullPath(appDirectory));
			}
			return _appDirectory;
		}

		public static void SetAppDirectory(string value)
		{
			_appDirectory = CheckAppDirectory(value);
		}

		public static UserConfig GetUserConfig()
		{
			if (_userConfig == null)
			{
				PerformanceTesting.Checkpoint("Begin Configuration");
				var defaultConfigFileProvider = new StringFilerProvider(Properties.Resources.default_user_yml);
				_userConfig = new ConfigurationBuilder()
					.AddYamlFile(defaultConfigFileProvider, "default.user.yml", false, false)
					.AddYamlFile(Path.Combine(Environment.UserHomeDirectory, ".config", "blent", "user.yml"), true, false)
					.Build()
					.Get<UserConfig>();
				PerformanceTesting.Checkpoint("End Configuration");
			}
			return _userConfig;
		}

		private static string CheckAppDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				Output.Logger.Fatal("AppDirectory does not exist", new { path });
				throw new FatalException($"AppDirectory [{path}] does not exist");
			};
			return path;
		}
	}
}

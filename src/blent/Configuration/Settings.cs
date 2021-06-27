using Blent.Configuration.Models;
using Blent.Utility;
using Blent.Utility.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Blent.Configuration
{
	public static class Settings
	{
		public static UserConfig UserConfig { get; private set; }

		public static string GetAppDirectory()
		{
			return Path.GetFullPath(UserConfig.AppDirectory.Replace("~", Environment.GetUserHomeDirectory()));
		}

		public static void InitUserConfig(ILogger logger)
		{
			PerformanceTesting.Checkpoint("Begin Configuration");

			logger.Debug("building user configuration");
			UserConfig = BuildUserConfig();

			PerformanceTesting.Checkpoint("End Configuration");
		}

		private static UserConfig BuildUserConfig()
		{
			var defaultConfigFileProvider = new StringFileProvider(Properties.Resources.default_user_yml);

			return new ConfigurationBuilder()
				.AddYamlFile(defaultConfigFileProvider, "default.user.yml", false, false)
				.AddYamlFile(Path.Combine(Environment.GetUserHomeDirectory(), ".config", "blent", "user.yml"), true, false)
				.Build()
				.Get<UserConfig>();
		}
	}
}

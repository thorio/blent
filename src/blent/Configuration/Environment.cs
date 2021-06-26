using Blent.Utility;
using System;
using System.Runtime.InteropServices;

namespace Blent.Configuration
{
	public static class Environment
	{
		private static Platform? _platform;

		public static Platform CurrentPlatform =>
			_platform ??= DeterminePlatform();

		public static string UserHomeDirectory =>
			System.Environment.GetEnvironmentVariable(GetPlatformDependentValue("HOME", "USERPROFILE"));

		public static T GetPlatformDependentValue<T>(T unixValue, T windowsValue) =>
			GetPlatformDependentValue(unixValue, windowsValue, unixValue);

		public static T GetPlatformDependentValue<T>(T linuxValue, T windowsValue, T osxValue)
		{
			return CurrentPlatform switch
			{
				Platform.Linux => linuxValue,
				Platform.Windows => windowsValue,
				Platform.OSX => osxValue,
				_ => throw new PlatformNotSupportedException(),
			};
		}

		private static Platform DeterminePlatform()
		{
			var platformMappings = new[] {
				(OSPlatform.Linux, Platform.Linux),
				(OSPlatform.Windows, Platform.Windows),
				(OSPlatform.OSX, Platform.OSX)
			};

			foreach (var (platform, enumValue) in platformMappings)
			{
				if (RuntimeInformation.IsOSPlatform(platform))
				{
					return enumValue;
				}
			}

			Output.Logger.Fatal("Unable to determine Platform");
			return ErrorPrinter.FatalAndQuit<Platform>("Unable to determine Platform");
		}
	}
}

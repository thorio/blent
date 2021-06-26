using Blent.Utility;
using System;
using System.Runtime.InteropServices;

namespace Blent.Configuration
{
	public static class Environment
	{
		private static readonly Lazy<Platform> _platform = new(DeterminePlatform);

		public static Platform GetCurrentPlatform() => _platform.Value;

		public static string UserHomeDirectory =>
			System.Environment.GetEnvironmentVariable(GetPlatformDependentValue("HOME", "USERPROFILE"));

		public static T GetPlatformDependentValue<T>(T unixValue, T windowsValue) =>
			GetPlatformDependentValue(unixValue, windowsValue, unixValue, unixValue);

		public static T GetPlatformDependentValue<T>(T linuxValue, T windowsValue, T osxValue, T freeBsdValue)
		{
			return GetCurrentPlatform() switch
			{
				Platform.Linux => linuxValue,
				Platform.Windows => windowsValue,
				Platform.OSX => osxValue,
				Platform.FreeBSD => freeBsdValue,
				_ => throw new PlatformNotSupportedException(),
			};
		}

		private static Platform DeterminePlatform()
		{
			var platformMappings = new[] {
				(OSPlatform.Linux, Platform.Linux),
				(OSPlatform.Windows, Platform.Windows),
				(OSPlatform.FreeBSD, Platform.FreeBSD),
				(OSPlatform.OSX, Platform.OSX)
			};

			foreach (var (platform, enumValue) in platformMappings)
			{
				if (RuntimeInformation.IsOSPlatform(platform))
				{
					return enumValue;
				}
			}

			Output.Logger.Fatal("unable to determine platform", new
			{
				runtime_identifier = RuntimeInformation.RuntimeIdentifier,
				process_architecture = RuntimeInformation.ProcessArchitecture,
				os_architecture = RuntimeInformation.OSArchitecture,
				os_description = RuntimeInformation.OSDescription,
				framework_description = RuntimeInformation.FrameworkDescription,
			});

			throw new FatalException($"Unable to determine Platform. \n" +
				$"{nameof(RuntimeInformation.RuntimeIdentifier),-22}:  {RuntimeInformation.RuntimeIdentifier}\n" +
				$"{nameof(RuntimeInformation.ProcessArchitecture),-22}:  {RuntimeInformation.ProcessArchitecture}\n" +
				$"{nameof(RuntimeInformation.OSArchitecture),-22}:  {RuntimeInformation.OSArchitecture}\n" +
				$"{nameof(RuntimeInformation.OSDescription),-22}:  {RuntimeInformation.OSDescription}\n" +
				$"{nameof(RuntimeInformation.FrameworkDescription),-22}:  {RuntimeInformation.FrameworkDescription}"
			);
		}
	}
}

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Blent.Configuration
{
	public static class YamlParser
	{
		private static IDeserializer _deserializer;

		public static T Deserialize<T>(string yaml)
		{
			return GetDeserializer().Deserialize<T>(yaml);
		}

		private static IDeserializer GetDeserializer()
		{
			if (_deserializer == null)
			{
				_deserializer = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();
			}
			return _deserializer;
		}
	}
}

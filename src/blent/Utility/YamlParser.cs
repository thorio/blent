using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Blent.Utility
{
	public static class YamlParser
	{
		private static IDeserializer _deserializer;
		private static ISerializer _serializer;

		public static T Deserialize<T>(string yaml)
		{
			return GetDeserializer().Deserialize<T>(yaml);
		}

		public static string Serialize<T>(T model)
		{
			return GetSerializer().Serialize(model);
		}

		private static IDeserializer GetDeserializer()
		{
			return _deserializer ??= new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();
		}

		private static ISerializer GetSerializer()
		{
			return _serializer ??= new SerializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
				.Build();
		}
	}
}

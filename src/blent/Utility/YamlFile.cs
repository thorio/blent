using System;
using System.IO;

namespace Blent.Utility
{
	public class YamlFile<T> : IDisposable where T : class
	{
		private readonly FileStream _stream;
		private readonly StreamReader _reader;
		private readonly StreamWriter _writer;

		private YamlFile(string path)
		{
			_stream = File.Open(path, FileMode.OpenOrCreate);
			_reader = new StreamReader(_stream);
			_writer = new StreamWriter(_stream);
		}

		public static T Read(string path)
		{
			using var file = new YamlFile<T>(path);
			return file.Read();
		}

		public static YamlFile<T> Open(string path)
		{
			return new YamlFile<T>(path);
		}

		public T Read()
		{
			_stream.Position = 0;
			var yaml = _reader.ReadToEnd();
			return YamlParser.Deserialize<T>(yaml);
		}

		public void Write(T model)
		{
			var yaml = YamlParser.Serialize(model);
			_stream.Position = 0;
			_stream.SetLength(0);
			_writer.Write(yaml);
			_writer.Flush();
		}

		public void Dispose()
		{
			_stream?.Dispose();
		}
	}
}

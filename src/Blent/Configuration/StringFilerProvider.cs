using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;

namespace Blent.Configuration
{
	public class StringFilerProvider : IFileProvider
	{
		private readonly StringFileInfo _stringFileInfo;

		public StringFilerProvider(byte[] data)
		{
			_stringFileInfo = new StringFileInfo(data);
		}

		public IDirectoryContents GetDirectoryContents(string subpath) =>
			throw new NotImplementedException();

		public IFileInfo GetFileInfo(string subpath) =>
			_stringFileInfo;

		public IChangeToken Watch(string filter) =>
			throw new NotImplementedException();

		private class StringFileInfo : IFileInfo
		{
			private readonly byte[] _data;

			public StringFileInfo(byte[] data)
			{
				_data = data;
			}

			public bool Exists => true;
			public long Length => _data.Length;
			public string PhysicalPath => null;
			public string Name => $"byteArray[{Length}]";
			public DateTimeOffset LastModified => throw new NotImplementedException();
			public bool IsDirectory => false;

			public Stream CreateReadStream() =>
				new MemoryStream(_data);
		}
	}
}

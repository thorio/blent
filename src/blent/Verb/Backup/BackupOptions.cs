using CommandLine;
using System.Collections.Generic;
using System.IO.Compression;

namespace Blent.Verb.Backup
{
	[Verb("backup", HelpText = "Back up stacks to zip archives.")]
	public class BackupOptions : Options
	{
		[Value(0, HelpText = "Stack(s) to back up. Leave blank to back up all stacks.", MetaName = "STACK...")]
		public IEnumerable<string> Stacks { get; set; }

		[Option('o', "output-dir", Required = true, HelpText = "The directory to save the archives in.")]
		public string BackupDirectory { get; set; }

		[Option('k', "keep", Default = 2, HelpText = "The number of archives to keep.")]
		public int NumberOfBackups { get; set; }

		[Option('c', "compression", Default = CompressionLevel.NoCompression, HelpText = "The compression level to use.")]
		public CompressionLevel CompressionLevel { get; set; }
	}
}

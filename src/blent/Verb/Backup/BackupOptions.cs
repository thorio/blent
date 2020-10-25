using CommandLine;
using System.Collections.Generic;
using System.IO.Compression;

namespace Blent.Verb.Backup
{
	[Verb("backup", HelpText = "Back up projects to zip archives.")]
	public class BackupOptions : Options
	{
		[Value(0, HelpText = "Project(s) to back up. Leave blank to back up all projects.", MetaName = "PROJECT...")]
		public IEnumerable<string> Projects { get; set; }

		[Option('o', "output-dir", Required = true, HelpText = "The directory to save the archives in.")]
		public string BackupDirectory { get; set; }

		[Option('k', "keep", Default = 2, HelpText = "The number of archives to keep.")]
		public int NumberOfBackups { get; set; }

		[Option('c', "compression", Default = CompressionLevel.NoCompression, HelpText = "The compression level to use.")]
		public CompressionLevel CompressionLevel { get; set; }
	}
}

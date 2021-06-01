using Blent.Utility.Drawing;
using Blent.Utility.Logging;
using System;

namespace Blent.Utility
{
	public static class Output
	{
		static Output()
		{
			Out = new OutputWriter(Console.Out);
			Error = new OutputWriter(Console.Error);
			Logger = new LogfmtLogger(new TextWriterLogTarget(Console.Out));
			Out.Enabled = Error.Enabled = true;
			Logger.Enabled = true;
		}

		public static OutputWriter Out { get; }
		public static OutputWriter Error { get; }
		public static LogfmtLogger Logger { get; }
	}
}

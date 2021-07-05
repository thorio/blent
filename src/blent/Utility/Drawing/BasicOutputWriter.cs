using System;

namespace Blent.Utility.Drawing
{
	public class BasicOutputWriter
	{
		protected IOutputTarget _target;

		public BasicOutputWriter(IOutputTarget textWriter, bool enabled = true)
		{
			_target = textWriter;
			Enabled = enabled;
		}

		public bool Enabled { get; set; }

		public virtual void Write(string text)
		{
			if (!Enabled) return;
			_target.Write(text.NormalizeLineEndings());
		}

		public virtual void WriteLine()
		{
			if (!Enabled) return;
			_target.Write(Environment.NewLine);
		}

		public virtual void WriteLine(string text)
		{
			if (!Enabled) return;
			_target.Write(text.NormalizeLineEndings() + Environment.NewLine);
		}
	}
}

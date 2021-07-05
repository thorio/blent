using System;

namespace Blent.Utility.Drawing
{
	public class OutputWriter
	{
		private IOutputTarget _target;
		private bool _buffering;

		public OutputWriter(IOutputTarget textWriter, bool enabled = true)
		{
			_target = textWriter;
			Enabled = enabled;
			Colors = new ColorHelper();
		}

		public ColorHelper Colors { get; set; }
		public bool Enabled { get; set; }

		public void Write(string text, Color color = Color.Default)
		{
			if (!Enabled) return;
			_target.Write(Colors.Get(color) + text.NormalizeLineEndings());
		}

		public void WriteLine()
		{
			if (!Enabled) return;
			_target.Write(Environment.NewLine);
		}

		public void WriteLine(string text, Color color = Color.Default)
		{
			if (!Enabled) return;
			_target.Write(Colors.Get(color) + text.NormalizeLineEndings() + Environment.NewLine);
		}

		public void ResetStyling()
		{
			if (!Enabled) return;
			_target.Write(Colors.GetReset());
		}

		public ExclusiveHandle<OutputWriter> BeginExclusiveWrite()
		{
			if (_buffering) throw new InvalidOperationException("Exclusive write handle was already aquired");
			_buffering = true;

			var originalTarget = _target;
			var buffer = new BufferedOutputTarget(originalTarget);

			// buffer our own writes
			_target = buffer;

			return new ExclusiveHandle<OutputWriter>(new OutputWriter(originalTarget, Enabled), () =>
			{
				// when the handle is released, first flush our buffer
				buffer.Flush();
				// then restore our original Target
				_target = originalTarget;
				_buffering = false;
			});
		}
	}
}

using System.Text;

namespace Blent.Utility.Drawing
{
	public class BufferedOutputTarget : IOutputTarget
	{
		private readonly IOutputTarget _target;
		private readonly StringBuilder _buffer;

		public BufferedOutputTarget(IOutputTarget target)
		{
			_target = target;
			_buffer = new StringBuilder();
		}

		public void Write(string value) =>
			_buffer.Append(value);

		public void Flush() =>
			_target.Write(_buffer.ToString());
	}
}

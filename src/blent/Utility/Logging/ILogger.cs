using System;

namespace Blent.Utility.Logging
{
	public interface ILogger
	{
		void Debug(string message, object fields = null);
		void Error(string message, object fields = null);
		void Error(string message, Exception ex, object fields = null);
		void Fatal(string message, object fields = null);
		void Fatal(string message, Exception ex, object fields = null);
		void Info(string message, object fields = null);
		void Log(LogLevel level, string message, object fields = null);
		void Trace(string message, object fields = null);
		void Warn(string message, object fields = null);
		void Warn(string message, Exception ex, object fields = null);
		ILogger With(object fields);
	}
}

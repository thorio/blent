using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Blent.Utility.Logging
{
	public class LogfmtLogger : ILogger
	{
		private const string LevelKey = "level";
		private const string ThreadKey = "thread";
		private const string MessageKey = "msg";
		private const string TimeKey = "time";

		public LogfmtLogger(ILogTarget target, LogLevel level, object context = null)
		{
			_target = target;
			_level = level;
			_context = ReadObject(context);
		}

		private LogfmtLogger(LogfmtLogger parent, LogLevel level, KeyValuePair<string, string>[] context)
		{
			_parent = parent;
			_level = level;
			_context = context;
		}

		private readonly LogLevel _level;
		private readonly ILogTarget _target;
		private readonly LogfmtLogger _parent;
		private readonly KeyValuePair<string, string>[] _context;

		public void Trace(string message, object fields = null) => Log(LogLevel.Trace, message, fields);
		public void Debug(string message, object fields = null) => Log(LogLevel.Debug, message, fields);
		public void Info(string message, object fields = null) => Log(LogLevel.Info, message, fields);
		public void Warn(string message, object fields = null) => Log(LogLevel.Warn, message, fields);
		public void Error(string message, object fields = null) => Log(LogLevel.Error, message, fields);
		public void Fatal(string message, object fields = null) => Log(LogLevel.Fatal, message, fields);

		public void Warn(string message, Exception ex, object fields = null) => Log(LogLevel.Warn, message, ex, fields);
		public void Error(string message, Exception ex, object fields = null) => Log(LogLevel.Error, message, ex, fields);
		public void Fatal(string message, Exception ex, object fields = null) => Log(LogLevel.Fatal, message, ex, fields);

		public ILogger With(object fields)
		{
			if (!ShouldLog()) return new LogfmtLogger(null, LogLevel.None);

			return new LogfmtLogger(this, _level, ReadObject(fields));
		}

		public void Log(LogLevel level, string message, object fields = null)
		{
			if (!ShouldLog(level)) return;

			Log(level, message, ReadObject(fields));
		}

		public void Log(LogLevel level, string message, Exception ex, object fields = null)
		{
			if (!ShouldLog(level)) return;

			Log(level, message, PreprocessException(ex).Concat(ReadObject(fields)).ToArray());
		}

		private void Log(LogLevel level, string message, params KeyValuePair<string, string>[] fields)
		{
			if (_context.Length > 0)
			{
				fields = _context.Concat(fields).ToArray();
			}

			if (_parent != null)
			{
				_parent.Log(level, message, fields);
			}
			else if (_target != null)
			{
				LogToTarget(level, message, fields);
			}
		}

		private void LogToTarget(LogLevel level, string message, params KeyValuePair<string, string>[] fields)
		{
			var allFields = new[] {
				Pair(TimeKey, DateTime.Now.ToString("s")),
				Pair(LevelKey, level.ToString().ToLower()),
				Pair(ThreadKey, Thread.CurrentThread.ManagedThreadId.ToString()),
				Pair(MessageKey, message),
			}
				.Concat(fields)
				.ToArray();

			_target.WriteLine(FormatLogLine(allFields));
		}

		private static string FormatLogLine(params KeyValuePair<string, string>[] fields)
		{
			var builder = new StringBuilder();
			foreach (var field in fields)
			{
				if (string.IsNullOrWhiteSpace(field.Value)) continue;

				var value = PreprocessValue(field.Value);
				builder.Append($"{field.Key}={value} ");
			}

			// trim last space
			builder.Length--;
			return builder.ToString();
		}

		private static KeyValuePair<string, string>[] ReadObject(object fields)
		{
			if (fields == null)
			{
				return Array.Empty<KeyValuePair<string, string>>();
			}

			return fields.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Select(p => Pair(p.Name, p.GetValue(fields)?.ToString()))
				.ToArray();
		}

		private static string PreprocessValue(string value)
		{
			value = value
				.Replace("\\", "\\\\")
				.Replace("\"", "\\\"")
				.Replace("\n", "\\n")
				.Replace("\r", "\\r");

			if (value.Contains(' ') || value.Contains('='))
			{
				value = $"\"{value}\"";
			}

			return value;
		}

		private static KeyValuePair<string, string>[] PreprocessException(Exception ex)
		{
			return new[] {
				Pair("exception", ex.GetType().FullName),
				Pair("exception_message", ex.Message),
				Pair("stacktrace", ex.StackTrace),
			};
		}

		private static KeyValuePair<string, string> Pair(string key, string value) =>
			new(key, value);

		private bool ShouldLog(LogLevel level) =>
			 ShouldLog() && _level <= level;

		private bool ShouldLog() =>
			_target != null || _parent != null;
	}
}

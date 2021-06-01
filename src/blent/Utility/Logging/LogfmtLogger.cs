using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Blent.Utility.Logging
{
	public class LogfmtLogger
	{
		private const string LevelKey = "level";
		private const string MessageKey = "msg";
		private const string TimeKey = "time";

		public LogfmtLogger(ILogTarget target)
		{
			Target = target;
		}

		public ILogTarget Target { get; set; }
		public bool Enabled { get; set; }

		public void Trace(string message, object fields) => Log(LogLevel.Trace, message, fields);
		public void Debug(string message, object fields) => Log(LogLevel.Debug, message, fields);
		public void Info(string message, object fields) => Log(LogLevel.Info, message, fields);
		public void Warn(string message, object fields) => Log(LogLevel.Warn, message, fields);
		public void Error(string message, object fields) => Log(LogLevel.Error, message, fields);
		public void Fatal(string message, object fields) => Log(LogLevel.Fatal, message, fields);

		public void Log(LogLevel level, string message, object fields)
		{
			if (!Enabled) return;

			Log(level, message, ReadObject(fields));
		}

		private void Log(LogLevel level, string message, params KeyValuePair<string, string>[] fields)
		{
			if (!Enabled) return;

			var allFields = new[] {
				new KeyValuePair<string, string>(TimeKey, DateTime.Now.ToString("s")),
				new KeyValuePair<string, string>(LevelKey, level.ToString().ToLower()),
				new KeyValuePair<string, string>(MessageKey, message),
			}
				.Concat(fields)
				.ToArray();

			Target?.WriteLine(FormatLogLine(allFields));
		}

		private KeyValuePair<string, string>[] ReadObject(object fields)
		{
			return fields.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Select(p => new KeyValuePair<string, string>(p.Name, p.GetValue(fields).ToString()))
				.ToArray();
		}

		private string FormatLogLine(params KeyValuePair<string, string>[] fields)
		{
			var builder = new StringBuilder();
			foreach (var field in fields)
			{
				var value = PreprocessValue(field.Value);
				builder.Append($"{field.Key}={value} ");
			}

			// trim last space
			builder.Length--;
			return builder.ToString();
		}

		private string PreprocessValue(string value)
		{
			value = value
				.Replace("\\", "\\\\")
				.Replace("\"", "\\\"")
				.Replace("\n", "\\n");

			if (value.Contains(' '))
			{
				value = $"\"{value}\"";
			}

			return value;
		}
	}
}

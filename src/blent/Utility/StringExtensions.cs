using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blent.Utility
{
	public static class StringExtensions
	{
		public static string NormalizeLineEndings(this string str)
		{
			return Regex.Replace(str, "\r\n|\n", Environment.NewLine);
		}

		public static string EscapeDoubleQuotes(this string str)
		{
			return str.Replace("\"", "\\\"");
		}

		public static IEnumerable<string> AsList(this string str, string seperator)
		{
			return str.NormalizeLineEndings()
				.Split(seperator)
				.Where(s => !string.IsNullOrEmpty(s));
		}
	}
}

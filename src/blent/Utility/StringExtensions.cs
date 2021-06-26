using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blent.Utility
{
	public static class StringExtensions
	{
		/// <summary>
		/// Replaces all occurrences of <b>\r\n</b> or <b>\n</b> with <see cref="Environment.NewLine"/>
		/// </summary>
		public static string NormalizeLineEndings(this string str)
		{
			return Regex.Replace(str, "\r\n|\n", Environment.NewLine);
		}

		/// <summary>
		/// Replaces all occurrences of <b>"</b> with <b>\"</b>
		/// </summary>
		public static string EscapeDoubleQuotes(this string str)
		{
			return str.Replace("\"", "\\\"");
		}

		/// <summary>
		/// Splits string using separator and discards any empty values
		/// </summary>
		public static IEnumerable<string> AsList(this string str, string seperator)
		{
			return str.NormalizeLineEndings()
				.Split(seperator)
				.Where(s => !string.IsNullOrEmpty(s));
		}
	}
}

using System;

namespace Blent.Utility
{
	public static class DateTimeExtensions
	{
		public static long ToUnixTimestamp(this DateTime time) =>
			((DateTimeOffset)time).ToUnixTimeSeconds();

		public static long ToUnixTimestampMillis(this DateTime time) =>
			((DateTimeOffset)time).ToUnixTimeMilliseconds();
	}
}

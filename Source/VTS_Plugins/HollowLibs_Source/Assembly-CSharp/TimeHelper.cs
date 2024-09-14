using System;
public static class TimeHelper
{
	private static readonly DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public static long UnixTimestampMilliseconds()
	{
		return (long)(DateTime.UtcNow - TimeHelper.epochStart).TotalMilliseconds;
	}
}

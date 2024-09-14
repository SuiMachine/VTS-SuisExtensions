public static class StringHelper
{
	public static bool IsNullOrEmptyOrWhitespace(this string s)
	{
		return string.IsNullOrEmpty(s) || string.IsNullOrEmpty(s.Trim());
	}
}
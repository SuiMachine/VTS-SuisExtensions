using SuisApiExtension;

public static class VTSPluginExternals
{
	public static void LogError(string error) => Plugin.LogError(error);

	public static void LogMessage(string message) => Plugin.LogMessage(message);

	public static void LogWarning(string message) => Plugin.LogWarning(message);
}
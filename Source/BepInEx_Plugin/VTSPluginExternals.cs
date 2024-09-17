using SuisApiExtension;

public static class VTSPluginExternals
{
	public static float CurrentMin_x { get; private set; }
	public static float CurrentMax_x { get; private set; }

	internal static void SetCurrentMinMax(float min, float max)
	{
		CurrentMin_x = min;
		CurrentMax_x = max;
	}

	public static void LogError(string error) => Plugin.LogError(error);

	public static void LogMessage(string message) => Plugin.LogMessage(message);

	public static void LogWarning(string message) => Plugin.LogWarning(message);
}
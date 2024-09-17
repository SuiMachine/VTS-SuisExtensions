using UnityEngine;

public static class VTSPluginExternals
{
	public static float CurrentMin_x { get; private set; }
	public static float CurrentMax_x { get; private set; }

	internal static void SetCurrentMinMax(float min, float max)
	{
		CurrentMin_x = min;
		CurrentMax_x = max;
	}

	public static void LogError(string error)
	{
		//This has different content in actual DLL;
		Debug.LogError(error);
	}

	public static void LogMessage(string message)
	{
		//This has different content in actual DLL;
		Debug.Log(message);
	}

	public static void LogWarning(string message)
	{
		//This has different content in actual DLL;
		Debug.LogWarning(message);
	}
}
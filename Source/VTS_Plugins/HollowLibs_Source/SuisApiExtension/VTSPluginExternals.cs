using UnityEngine;

public static class VTSPluginExternals
{
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
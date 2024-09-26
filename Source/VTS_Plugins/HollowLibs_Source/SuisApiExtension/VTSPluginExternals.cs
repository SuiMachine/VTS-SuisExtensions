using UnityEngine;

public static class VTSPluginExternals
{
	public static float CurrentMin_x { get; private set; }
	public static float CurrentMax_x { get; private set; }

	private static Camera m_Live2DCamera;
	public static Camera Live2DCamera
	{
		get
		{
			if(m_Live2DCamera == null)
			{
				var find = GameObject.FindAnyObjectByType<CameraResolutionManager>(FindObjectsInactive.Include);
				if(find != null)
				{
					m_Live2DCamera = find.GetComponent<Camera>();
					VTSPluginExternals.LogError($"Camera is: {m_Live2DCamera}");
				}
			}
			return m_Live2DCamera;
		}
	}

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
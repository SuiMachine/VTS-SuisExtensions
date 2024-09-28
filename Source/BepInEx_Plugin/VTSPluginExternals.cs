using SuisApiExtension;
using UnityEngine;

public static class VTSPluginExternals
{
	private static Camera m_Live2DCamera;
	public static Camera Live2DCamera
	{
		get
		{
			if (m_Live2DCamera == null)
			{
				CameraResolutionManager find = GameObject.FindAnyObjectByType<CameraResolutionManager>(FindObjectsInactive.Include);
				if (find != null)
				{
					Transform subNode = find.transform.Find("Live2D Camera");
					if(subNode != null)
					{
						m_Live2DCamera = subNode.GetComponent<Camera>();
					}					
				}
			}
			return m_Live2DCamera;
		}
	}

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
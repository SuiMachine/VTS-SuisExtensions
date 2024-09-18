using UnityEngine;

public class CameraResolutionManager : MonoBehaviour
{
	private int mCameraFrameWidth = -1;
	private int mCameraFrameHeight = -1;

	public Vector2Int GetResolution()
	{
		return new Vector2Int(this.mCameraFrameWidth, this.mCameraFrameHeight);
	}

	public Vector2Int GetScreenResolutionARBackgroundRenderer()
	{
		return new Vector2Int(Screen.height, Screen.width);
	}
}
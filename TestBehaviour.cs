using UnityEngine;

namespace SuisApiExtension
{
	public class TestBehaviour : MonoBehaviour
	{
		private void Start()
		{
			StartCoroutine(Dupa());
		}

		private System.Collections.IEnumerator Dupa()
		{
			while (VTubeStudioModelLoader.Instance() == null || VTubeStudioModelLoader.GetMainModel() == null || TwitchDropper.Instance() == null)
				yield return new WaitForSeconds(1);

			while (true)
			{
				TwitchDropper.Instance().DropImage("file://E:/discord-mark-white.png");
				TwitchDropper.Instance().DropImage("file://E:/discord-mark-white.png");
				TwitchDropper.Instance().DropImage("file://E:/discord-mark-white.png");
				TwitchDropper.Instance().DropImage("file://E:/discord-mark-white.png");
				TwitchDropper.Instance().DropImage("file://E:/discord-mark-white.png");
				TwitchDropper.Instance().DropImage("file://E:/discord-mark-white.png");
				TwitchDropper.Instance().DropImage("file://E:/discord-mark-white.png");
				TwitchDropper.Instance().DropImage("file://E:/discord-mark-white.png");
				TwitchDropper.Instance().DropImage("file://E:/discord-mark-white.png");
				TwitchDropper.Instance().DropImage("file://E:/discord-mark-white.png");

				yield return new WaitForSeconds(1);
			}
		}

		private void OnGUI()
		{
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label("No i kurwa dobrze");
			GUILayout.EndHorizontal();
		}
	}
}

using HarmonyLib;
using UnityEngine.Networking;

namespace SuisApiExtension
{
	[HarmonyPatch]
	public static class TestDetours
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(CachedImageNormalOrAnimated), "CreateOrGetCached")]
		public static void Detoured(string id, DownloadHandlerTexture dh)
		{
			Plugin.LogMessage($"Shit: {id}");
		}
	}
}

using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace SuisApiExtension.Detour
{
	[HarmonyPatch]
	public static class TwitchDropper_Detour
	{

		[HarmonyPostfix]
		[HarmonyPatch(typeof(TwitchDropper), "Start")]
		private static void Start(ref PolygonCollider2D ___ModelPolyCollider)
		{
			___ModelPolyCollider.enabled = true;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(TwitchDropper), "Update")]
		private static bool DetouredUpdate(TwitchDropper __instance, ref float ___currentMin_x, ref float ___currentMax_x, ref Vector2 ___currentCenter)
		{
			__instance.cacheCount = CachedImageNormalOrAnimated.GetCurrentCacheSize();
			__instance.allCount = CachedImageNormalOrAnimated.GetAllExistingElemetsCount();
			List<Vector2> currentModelHull = SingletonMonoBehaviour<ModelMeshHullChecker>.Instance().GetCurrentModelHull(false, true);
			float tempMin = float.MaxValue;
			float tempMax = float.MinValue;
			___currentMin_x = 0f;
			___currentMax_x = 0f;
			if (currentModelHull != null && currentModelHull.Count >= 3)
			{
				___currentCenter = Handler_ModelOutlineEvent.GetCenter(currentModelHull);
				Vector2[] hullVerts = new Vector2[currentModelHull.Count];
				float softBoundMult = 0.98f;
				int it = 0;
				bool found = false;
				foreach (Vector2 vector in currentModelHull)
				{
					if (vector.x < tempMin)
					{
						found = true;
						tempMin = vector.x;
					}
					if (vector.x > tempMax)
					{
						found = true;
						tempMax = vector.x;
					}
					hullVerts[it] = ___currentCenter * (1f - softBoundMult) + vector * softBoundMult;
					it++;
				}
				___currentMin_x = (found ? tempMin : 0f);
				___currentMax_x = (found ? tempMax : 0f);
				VTSPluginExternals.SetCurrentMinMax(___currentMin_x, ___currentMax_x);

				__instance.ModelPolyCollider.points = hullVerts;
			}

			return false;
		}
	}
}

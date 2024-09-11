using UnityEditor;
using UnityEngine;

namespace Assets
{
#if UNITY_EDITOR
	public class BuildAssetBundle : MonoBehaviour
	{
		[MenuItem("Example/Build content")]
		static void BuildBundles()
		{
			BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
		}
	}
#endif
}

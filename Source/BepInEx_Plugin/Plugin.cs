using BepInEx;
using BepInEx.Unity.Mono;
using UnityEngine;

namespace SuisApiExtension;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
	private static BepInEx.Logging.ManualLogSource LoggerInstance;
	private static HarmonyLib.Harmony HarmonyInstance;

	private void Awake()
	{
		LoggerInstance = Logger;
		HarmonyInstance = new HarmonyLib.Harmony("local.SuiApiExtension.SuiMachine");
		Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
	}

	private void Start()
	{
		HarmonyInstance.PatchAll();
		LoadBundle();
	}

	private void LoadBundle()
	{
		var loaded = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/testbundle.asset");
		if (loaded == null)
		{
			LoggerInstance.LogMessage("Kurwa");
			return;
		}

		var assets = loaded.LoadAllAssets<GameObject>();
		if(assets.Length > 0)
		{
			LoggerInstance.LogMessage(assets.Length);
		}

		var asset = loaded.LoadAsset<GameObject>("TestObject.prefab");
		if (asset == null)
		{
			LoggerInstance.LogMessage("No test object");
			return;
		}

		var script = asset.GetComponent<ExtendedImagesDropper>();

		LoggerInstance.LogMessage($"Image dropper is: {script != null}");

		var instance = GameObject.Instantiate(script);
		DontDestroyOnLoad(instance.gameObject);
		LoggerInstance.LogMessage("Stuff is ok");

		loaded.Unload(false);
	}

	public static void LogMessage(string text) => LoggerInstance.LogMessage(text);

	public static void LogError(string text) => LoggerInstance.LogError(text);

	public static void LogWarning(string text) => LoggerInstance.LogWarning(text);

}

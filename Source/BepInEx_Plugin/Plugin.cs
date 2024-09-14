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
	}

	public static void LogMessage(string text) => LoggerInstance.LogMessage(text);

	public static void LogError(string text) => LoggerInstance.LogError(text);

	public static void LogWarning(string text) => LoggerInstance.LogWarning(text);

}

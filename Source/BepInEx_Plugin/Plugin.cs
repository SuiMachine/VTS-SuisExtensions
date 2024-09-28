using BepInEx;
using BepInEx.Unity.Mono;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SuisApiExtension;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
	private static BepInEx.Logging.ManualLogSource LoggerInstance;
	private static HarmonyLib.Harmony HarmonyInstance;
	internal static List<Assembly> LibsLoaded = new List<Assembly>();

	private void Awake()
	{
		LoggerInstance = Logger;
		HarmonyInstance = new HarmonyLib.Harmony("local.SuiApiExtension.SuiMachine");
		Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

		LoadExtensionDLLs();
	}

	private void LoadExtensionDLLs()
	{
		var path = Path.Combine(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName, "VTSExtensions");
		if (!Directory.Exists(path))
			return;

		var files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
        foreach (var item in files)
        {
			try
			{
				LibsLoaded.Add(Assembly.LoadFile(item));
				Logger.LogMessage($"Loaded extension  {Path.GetFileName(item)}");
			}
			catch(Exception e)
			{
				Logger.LogError($"Failed to load {Path.GetFileName(item)}: {e}");
			}
		}
	}

	private void Start()
	{
		HarmonyInstance.PatchAll();
	}

	public static void LogMessage(string text) => LoggerInstance.LogMessage(text);

	public static void LogError(string text) => LoggerInstance.LogError(text);

	public static void LogWarning(string text) => LoggerInstance.LogWarning(text);

}

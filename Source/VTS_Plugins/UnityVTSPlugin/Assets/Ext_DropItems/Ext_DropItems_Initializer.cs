using Assets.Ext_DropItems.Scripts;
using Assets.ExtendedDropImages.Messages;
using SuisApiExtension.API;
using System.IO;
using UnityEngine;

namespace Assets.ExtendedDropImages
{
	public class ExtendedDropImagesInitializer
	{
		[VTSExtension_ExecuteAtApiStart]
		public static void Initialize()
		{
			var path = Path.Combine("BepInEx", "plugins", "VTSExtensions", "Assets", "ext_imagedropper.assetbundle");
			if (!File.Exists(path))
			{
				VTSPluginExternals.LogError("No ext_imagedropper.assetbundle found");
				return;
			}
			var assetBundleLoader = AssetBundle.LoadFromFile(path);
			var dropper = assetBundleLoader.LoadAsset<GameObject>("Ext_ImageDropper");
			if (dropper == null)
			{
				VTSPluginExternals.LogError("No dropper object found");
				return;
			}

			var dropperInstance = GameObject.Instantiate(dropper);
			var dropperComponent = dropperInstance.GetComponent<Ext_ImageDropper>();
			if(dropperComponent == null)
			{
				VTSPluginExternals.LogError("No dropper component inside of an object instance found");
				return;
			}

			dropperComponent.gameObject.name = nameof(Ext_ImageDropper);

			var executor = new GameObject(nameof(Ext_APIExecutor_DropItems)).AddComponent<Ext_APIExecutor_DropItems>();
			if (!APIExecutorsExtended.RegisterCustomExecutor<ExtendedDropItemRequest>(executor, true))
				VTSPluginExternals.LogError($"Failed to register {nameof(ExtendedDropItemRequest)}");
			assetBundleLoader.Unload(false);
		}
	}
}

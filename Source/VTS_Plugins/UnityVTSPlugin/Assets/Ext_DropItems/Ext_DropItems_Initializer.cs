using Assets.Ext_DropItems.Scripts;
using Assets.ExtendedDropImages.Messages;
using SuisApiExtension.API;
using UnityEngine;

namespace Assets.ExtendedDropImages
{
	public class ExtendedDropImagesInitializer
	{
		[VTSExtension_ExecuteAtApiStart]
		public static void Initialize()
		{
			var dropperObject = new GameObject(nameof(Ext_ImageDropper));
			dropperObject.AddComponent<Ext_ImageDropper>();


			GameObject go = new GameObject(nameof(Ext_APIExecutor_DropItems));
			Ext_APIExecutor_DropItems exector = go.AddComponent<Ext_APIExecutor_DropItems>();
			if(!APIExecutorsExtended.RegisterCustomExecutor<ExtendedDropItemRequest>(exector, true))
				VTSPluginExternals.LogError($"Failed to register {nameof(ExtendedDropItemRequest)}");
		}
	}
}

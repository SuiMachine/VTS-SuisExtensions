using Assets.ExtendedDropImages.Messages;
using SuisApiExtension.Detour;
using UnityEngine;
namespace SuisApiExtension.API
{
	public class Ext_APIExecutor_DropItems : IAPIRequestCustomExecutor
	{
		protected override void ExecuteInternal(APICustomMessage payload)
		{
			var deserializedData = payload.data.ToObject<ExtendedDropItemRequest>();

			if (deserializedData == null || string.IsNullOrEmpty(deserializedData.fileName) || deserializedData.fileName.Trim() == "")
			{
				VTubeStudioAPI_Detour.SendCustomError(payload, ErrorID.ItemFileNameMissing, "You have to provide an ID of an Item to load.");
				return;
			}

			string pathToLoadFrom = System.IO.Path.Combine(Application.streamingAssetsPath, "Items", deserializedData.fileName).Replace('\\', '/');

			if (!System.IO.File.Exists(pathToLoadFrom))
			{
				VTubeStudioAPI_Detour.SendCustomError(payload, ErrorID.ItemFileNameNotFound, "No item to load was found.");
				return;
			}

			APIBaseMessage<ExtendedDropItemResponse> basicResponse = VTubeStudioAPI.GetBasicResponse<ExtendedDropItemResponse>(payload.websocketSessionID, payload.requestID, "ExtendedDropItemResponse");
			basicResponse.data = new ExtendedDropItemResponse();
			basicResponse.data.success = true;
			var twitch_Dropper = TwitchDropper.Instance();
			if (twitch_Dropper != null)
			{
				for (int i = 0; i < deserializedData.count; i++)
					twitch_Dropper.DropImage("file://" + pathToLoadFrom);
			}

			VTubeStudioAPI_Detour.sendToSession(basicResponse);
		}

		protected override string GetExecutorRequestName() => nameof(ExtendedDropItemRequest);

		protected override bool InitializeInternal()
		{
			return true;
		}
	}
}

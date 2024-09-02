using UnityEngine;

namespace SuisApiExtension.API
{
	public class Executor_ExtendedDropImageRequest : IAPIRequestExecutor<ExtendedDropItemRequest>
	{
		protected override void ExecuteInternal(APIBaseMessage<ExtendedDropItemRequest> payload)
		{
			if (payload.data == null || payload.data.fileName.IsNullOrEmptyOrWhitespace())
			{
				VTubeStudioAPI.SendError(payload, ErrorID.ItemFileNameMissing, "You have to provide an ID of an Item to load.");
				return;
			}

			string pathToLoadFrom = System.IO.Path.Combine(Application.streamingAssetsPath, "Items", payload.data.fileName).Replace('\\', '/');
			Plugin.LogError($"Path {pathToLoadFrom}");

			if (!System.IO.File.Exists(pathToLoadFrom))
			{
				VTubeStudioAPI.SendError(payload, ErrorID.ItemFileNameNotFound, "No item to load was found.");
				return;
			}

			Plugin.LogError($"Response?");

			APIBaseMessage<ExtendedDropItemResponse> basicResponse = VTubeStudioAPI.GetBasicResponse<ExtendedDropItemResponse>(payload.websocketSessionID, payload.requestID, "ExtendedDropItemResponse");
			basicResponse.data = new ExtendedDropItemResponse();
			basicResponse.data.success = true;

			VTubeStudioAPI.sendToSession<ExtendedDropItemResponse>(basicResponse);
		}

		protected override string GetExecutorRequestName() => nameof(ExtendedDropItemRequest);

		protected override bool InitializeInternal()
		{
			return true;
		}
	}
}

using System;
using System.Threading.Tasks;
using UnityEngine;

public class VTubeStudioAPI : MonoBehaviour
{
	internal void SendInternalError<T>(APIBaseMessage<T> payload) where T : IAPIMessage
	{
		ErrorID errorID = ErrorID.InternalServerError;
		string text = "Internal server error while processing request. Please check the VTube Studio logs and file a bug report in the VTube Studio Discord.";
		VTubeStudioAPI.sendToSession<APIError>(VTubeStudioAPI.createError(payload.websocketSessionID, payload.requestID, errorID, text, false));
	}

	public static APIBaseMessage<T> GetBasicResponse<T>(string websocketSessionID, string requestID, string messageType) where T : IAPIMessage
	{
		return new APIBaseMessage<T>
		{
			apiName = "VTubeStudioPublicAPI",
			apiVersion = "1.0",
			timestamp = TimeHelper.UnixTimestampMilliseconds(),
			requestID = requestID,
			messageType = messageType,
			websocketSessionID = websocketSessionID
		};
	}

	private static APIBaseMessage<APIError> createError(string websocketSessionID, string requestID, ErrorID errorID, string errorMessage, bool sendError)
	{
		APIBaseMessage<APIError> basicResponse = VTubeStudioAPI.GetBasicResponse<APIError>(websocketSessionID, requestID, "APIError");
		basicResponse.data = new APIError();
		basicResponse.data.errorID = errorID;
		basicResponse.data.message = errorMessage;
		if (sendError)
		{
			VTubeStudioAPI.sendToSession<APIError>(basicResponse);
		}
		return basicResponse;
	}

	public static async void sendToSession<T>(APIBaseMessage<T> responseToSend) where T : IAPIMessage
	{
		await Task.Run(delegate
		{
			//The internals are here in original DLL
		});
	}
}

public enum ErrorID
{
	InternalServerError,
	APIAccessDeactivated,
	JSONInvalid,
	APINameInvalid,
	APIVersionInvalid,
	RequestIDInvalid,
	RequestTypeMissingOrEmpty,
	RequestTypeUnknown,
	RequestRequiresAuthetication,
	RequestRequiresPermission,
	TokenRequestDenied = 50,
	TokenRequestCurrentlyOngoing,
	TokenRequestPluginNameInvalid,
	TokenRequestDeveloperNameInvalid,
	TokenRequestPluginIconInvalid,
	AuthenticationTokenMissing = 100,
	AuthenticationPluginNameMissing,
	AuthenticationPluginDeveloperMissing,
	ModelIDMissing = 150,
	ModelIDInvalid,
	ModelIDNotFound,
	ModelLoadCooldownNotOver,
	CannotCurrentlyChangeModel,
	HotkeyQueueFull = 200,
	HotkeyExecutionFailedBecauseNoModelLoaded,
	HotkeyIDNotFoundInModel,
	HotkeyCooldownNotOver,
	HotkeyIDFoundButHotkeyDataInvalid,
	HotkeyExecutionFailedBecauseBadState,
	HotkeyUnknownExecutionFailure,
	HotkeyExecutionFailedBecauseLive2DItemNotFound,
	HotkeyExecutionFailedBecauseLive2DItemsDoNotSupportThisHotkeyType,
	ColorTintRequestNoModelLoaded = 250,
	ColorTintRequestMatchOrColorMissing,
	ColorTintRequestInvalidColorValue,
	MoveModelRequestNoModelLoaded = 300,
	MoveModelRequestMissingFields,
	MoveModelRequestValuesOutOfRange,
	CustomParamNameInvalid = 350,
	CustomParamValuesInvalid,
	CustomParamAlreadyCreatedByOtherPlugin,
	CustomParamExplanationTooLong,
	CustomParamDefaultParamNameNotAllowed,
	CustomParamLimitPerPluginExceeded,
	CustomParamLimitTotalExceeded,
	CustomParamDeletionNameInvalid = 400,
	CustomParamDeletionNotFound,
	CustomParamDeletionCreatedByOtherPlugin,
	CustomParamDeletionCannotDeleteDefaultParam,
	InjectDataNoDataProvided = 450,
	InjectDataValueInvalid,
	InjectDataWeightInvalid,
	InjectDataParamNameNotFound,
	InjectDataParamControlledByOtherPlugin,
	InjectDataModeUnknown,
	ParameterValueRequestParameterNotFound = 500,
	NDIConfigCooldownNotOver = 550,
	NDIConfigResolutionInvalid,
	ExpressionStateRequestInvalidFilename = 600,
	ExpressionStateRequestFileNotFound,
	ExpressionActivationRequestInvalidFilename = 650,
	ExpressionActivationRequestFileNotFound,
	ExpressionActivationRequestNoModelLoaded,
	SetCurrentModelPhysicsRequestNoModelLoaded = 700,
	SetCurrentModelPhysicsRequestModelHasNoPhysics,
	SetCurrentModelPhysicsRequestPhysicsControlledByOtherPlugin,
	SetCurrentModelPhysicsRequestNoOverridesProvided,
	SetCurrentModelPhysicsRequestPhysicsGroupIDNotFound,
	SetCurrentModelPhysicsRequestNoOverrideValueProvided,
	SetCurrentModelPhysicsRequestDuplicatePhysicsGroupID,
	ItemFileNameMissing = 750,
	ItemFileNameNotFound,
	ItemLoadLoadCooldownNotOver,
	CannotCurrentlyLoadItem,
	CannotLoadItemSceneFull,
	ItemOrderInvalid,
	ItemOrderAlreadyTaken,
	ItemLoadValuesInvalid,
	ItemCustomDataInvalid,
	ItemCustomDataCannotAskRightNow,
	ItemCustomDataLoadRequestRejectedByUser,
	CannotCurrentlyUnloadItem = 800,
	ItemAnimationControlInstanceIDNotFound = 850,
	ItemAnimationControlUnsupportedItemType,
	ItemAnimationControlAutoStopFramesInvalid,
	ItemAnimationControlTooManyAutoStopFrames,
	ItemAnimationControlSimpleImageDoesNotSupportAnim,
	ItemMoveRequestInstanceIDNotFound = 900,
	ItemMoveRequestInvalidFadeMode,
	ItemMoveRequestItemOrderTakenOrInvalid,
	ItemMoveRequestCannotCurrentlyChangeOrder,
	EventSubscriptionRequestEventTypeUnknown = 950,
	ArtMeshSelectionRequestNoModelLoaded = 1000,
	ArtMeshSelectionRequestOtherWindowsOpen,
	ArtMeshSelectionRequestModelDoesNotHaveArtMesh,
	ArtMeshSelectionRequestArtMeshIDListError,
	ItemPinRequestGivenItemNotLoaded = 1050,
	ItemPinRequestInvalidAngleOrSizeType,
	ItemPinRequestModelNotFound,
	ItemPinRequestArtMeshNotFound,
	ItemPinRequestPinPositionInvalid,
	PermissionRequestUnknownPermission = 1100,
	PermissionRequestCannotRequestRightNow,
	PermissionRequestFileProblem,
	PostProcessingListReqestInvalidFilter = 1150,
	PostProcessingUpdateReqestCannotUpdateRightNow = 1200,
	PostProcessingUpdateRequestFadeTimeInvalid,
	PostProcessingUpdateRequestLoadingPresetAndValues,
	PostProcessingUpdateRequestPresetFileLoadFailed,
	PostProcessingUpdateRequestValueListInvalid,
	PostProcessingUpdateRequestValueListContainsDuplicates,
	PostProcessingUpdateRequestTriedToLoadRestrictedEffect,
	EVENT_OFFSET = 100000,
	Event_TestEvent_TestMessageTooLong = 100000,
	Event_ModelLoadedEvent_ModelIDInvalid = 100050,
	Event_HotkeyTriggeredEvent_HotkeyActionInvalid = 100100
}

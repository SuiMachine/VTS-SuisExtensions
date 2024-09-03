using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuisApiExtension.API;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SuisApiExtension.Detour
{
	[HarmonyPatch]
	public static class VTubeStudioAPI_Detour
	{
		private static VTubeStudioAPI instance;
		private static WebSocketServer wssv;
		private static ConcurrentQueue<string> apiLogsToPrintNextFrame;
		private static ConcurrentQueue<string> justOpenedSessionIDs;
		private static ConcurrentQueue<string> justClosedSessionIDs;
		private static ConcurrentQueue<APIBaseMessage<APIMessageEmpty>> inboundMessageQueue;
		private static ConcurrentDictionary<string, WebSocket> sessions;

		private static APIExecutors normalExecutor;
		private static APIExecutorsExtended extendedExecutor;

		private static MethodInfo messageTypeInvalidCall;


		[HarmonyPrefix]
		[HarmonyPatch(typeof(VTubeStudioAPI), "Start")]
		private static void StartDetoured(VTubeStudioAPI __instance)
		{
			normalExecutor = MonoBehaviour.FindObjectOfType<APIExecutors>();
			if (normalExecutor != null)
			{
				Plugin.LogMessage("Adding additional executor");
				extendedExecutor = normalExecutor.gameObject.AddComponent<APIExecutorsExtended>();

				var go = new GameObject("ExtendedDropImageRequest");
				go.transform.parent = extendedExecutor.transform;
				extendedExecutor.ExecutorInstance_ExtendedDropImageRequest = go.AddComponent<Executor_ExtendedDropImageRequest>();

			}
		}

		private static void PurgeReferences()
		{
			wssv = null;
			apiLogsToPrintNextFrame = null;
			justOpenedSessionIDs = null;
			justClosedSessionIDs = null;
			inboundMessageQueue = null;
			sessions = null;
		}

		private static void CheckReference()
		{
			if (wssv == null)
			{
				var tempWssv = (WebSocketServer)typeof(VTubeStudioAPI).GetField(nameof(wssv), BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				if (tempWssv != null)
				{
					PurgeReferences();
					wssv = tempWssv;
					Plugin.LogMessage("Got wssv");
				}
			}

			if (apiLogsToPrintNextFrame == null)
			{
				apiLogsToPrintNextFrame = (ConcurrentQueue<string>)typeof(VTubeStudioAPI).GetField(nameof(apiLogsToPrintNextFrame), BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				if (apiLogsToPrintNextFrame != null)
					Plugin.LogMessage("Got apiLogsToPrintNextFrame");
			}

			if (justOpenedSessionIDs == null)
			{
				justOpenedSessionIDs = (ConcurrentQueue<string>)typeof(VTubeStudioAPI).GetField(nameof(justOpenedSessionIDs), BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				if (justOpenedSessionIDs != null)
					Plugin.LogMessage("Got justOpenedSessionIDs");
			}

			if (justClosedSessionIDs == null)
			{
				justClosedSessionIDs = (ConcurrentQueue<string>)typeof(VTubeStudioAPI).GetField(nameof(justClosedSessionIDs), BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				if (justClosedSessionIDs != null)
					Plugin.LogMessage("Got justClosedSessionIDs");
			}

			if (inboundMessageQueue == null)
			{
				inboundMessageQueue = (ConcurrentQueue<APIBaseMessage<APIMessageEmpty>>)typeof(VTubeStudioAPI).GetField(nameof(inboundMessageQueue), BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				if (inboundMessageQueue != null)
					Plugin.LogMessage("Got inboundMessageQueue");
			}

			if(sessions == null)
			{
				sessions = (ConcurrentDictionary<string, WebSocket>)typeof(VTubeStudioAPI).GetField(nameof(sessions), BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				if (sessions != null)
					Plugin.LogMessage("Got sessions");
			}

			if (messageTypeInvalidCall == null)
			{
				messageTypeInvalidCall = typeof(VTubeStudioAPI).GetMethod("messageTypeInvalid", BindingFlags.Static | BindingFlags.NonPublic);
			}

		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(VTubeStudioAPI), "Update")]
		private static bool Update_Detour(VTubeStudioAPI __instance, ref int ___totalReceivedRequests)
		{
			if (instance != __instance)
			{
				PurgeReferences();
				instance = __instance;
			}
			CheckReference();

			if (VTubeStudioAPI_Detour.wssv == null || VTubeStudioAPI_Detour.apiLogsToPrintNextFrame == null || VTubeStudioAPI_Detour.justOpenedSessionIDs == null)
				return false;


			if (!VTubeStudioAPI_Detour.apiLogsToPrintNextFrame.IsEmpty && VTubeStudioAPI_Detour.apiLogsToPrintNextFrame.TryDequeue(out string logText) && logText != null)
			{
				VTubeStudioAPI.APIDebug(logText, false);
			}

			if (!VTubeStudioAPI_Detour.justOpenedSessionIDs.IsEmpty && VTubeStudioAPI_Detour.justOpenedSessionIDs.TryDequeue(out string justOpenedSessionIDText) && __instance.apiSessionOpenedClosedEvent != null)
			{
				__instance.apiSessionOpenedClosedEvent.Invoke(justOpenedSessionIDText, APISessionEvent.Opened);
			}

			if (!VTubeStudioAPI_Detour.justClosedSessionIDs.IsEmpty && VTubeStudioAPI_Detour.justClosedSessionIDs.TryDequeue(out string justClosedSessionText) && __instance.apiSessionOpenedClosedEvent != null)
			{
				__instance.apiSessionOpenedClosedEvent.Invoke(justClosedSessionText, APISessionEvent.Closed);
			}

			while (!VTubeStudioAPI_Detour.inboundMessageQueue.IsEmpty)
			{
				if (VTubeStudioAPI_Detour.inboundMessageQueue.TryDequeue(out APIBaseMessage<APIMessageEmpty> apibaseMessage))
				{
					string wholePayloadAsString = apibaseMessage.data.wholePayloadAsString;
					string messageType = apibaseMessage.messageType;
					string requestID = apibaseMessage.requestID;
					string websocketSessionID = apibaseMessage.websocketSessionID;
					AuthenticatedSession sessionAuthInfo = apibaseMessage.sessionAuthInfo;
					if (VTubeStudioAPI.doAPILog_DEBUG)
					{
						VTubeStudioAPI.APIDebug("[API][C->A] " + wholePayloadAsString, false);
					}

					if (messageType != null)
					{
						if (messageType.Length == 0)
						{
							//Plugin.LogMessage($"Messange lengh 0: {messageType}");

							messageTypeInvalidCall.Invoke(null, [websocketSessionID, requestID, messageType, sessionAuthInfo]);
							continue;
						}

						switch (messageType)
						{
							case "InputParameterListRequest":
								execute_InputParameterListRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "PostProcessingUpdateRequest":
								execute_PostProcessingUpdateRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "SceneColorOverlayInfoRequest":
								execute_SceneColorOverlayInfoRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "InjectParameterDataRequest":
								execute_InjectParameterDataRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "HotkeysInCurrentModelRequest":
								execute_HotkeysInCurrentModelRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "APIStateRequest":
								execute_APIStateRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "StatisticsRequest":
								execute_StatisticsRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "EventSubscriptionRequest":
								execute_EventSubscriptionRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ExpressionStateRequest":
								execute_ExpressionStateRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ItemUnloadRequest":
								execute_ItemUnloadRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ExpressionActivationRequest":
								execute_ExpressionActivationRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ItemPinRequest":
								execute_ItemPinRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "AvailableModelsRequest":
								execute_AvailableModelsRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ModelLoadRequest":
								execute_ModelLoadRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "PostProcessingListRequest":
								execute_PostProcessingListRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "AuthenticationTokenRequest":
								execute_AuthenticationTokenRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ParameterCreationRequest":
								execute_ParameterCreationRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ColorTintRequest":
								execute_ColorTintRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ParameterValueRequest":
								execute_ParameterValueRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "HotkeyTriggerRequest":
								execute_HotkeyTriggerRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ItemAnimationControlRequest":
								execute_ItemAnimationControlRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "FaceFoundRequest":
								execute_FaceFoundRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "SetCurrentModelPhysicsRequest":
								execute_SetCurrentModelPhysicsRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "Live2DParameterListRequest":
								execute_Live2DParameterListRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "LoadModelFromURLRequest":
								execute_LoadModelFromURLRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "PermissionRequest":
								execute_PermissionRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ArtMeshSelectionRequest":
								execute_ArtMeshSelectionRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "GetCurrentModelPhysicsRequest":
								execute_GetCurrentModelPhysicsRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "VTSFolderInfoRequest":
								execute_VTSFolderInfoRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "NDIConfigRequest":
								execute_NDIConfigRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "CurrentModelRequest":
								execute_CurrentModelRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ItemMoveRequest":
								execute_ItemMoveRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "AuthenticationRequest":
								execute_AuthenticationRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "MoveModelRequest":
								execute_MoveModelRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ParameterDeletionRequest":
								execute_ParameterDeletionRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ItemListRequest":
								execute_ItemListRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ItemLoadRequest":
								execute_ItemLoadRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ArtMeshListRequest":
								execute_ArtMeshListRequest(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							case "ExtendedDropItemRequest":
								execute_ExtendedDropImage(websocketSessionID, requestID, wholePayloadAsString, sessionAuthInfo);
								continue;
							default:
								messageTypeInvalidCall.Invoke(null, [websocketSessionID, requestID, messageType, sessionAuthInfo]);
								continue;
						}
					}
				}
			}
			return false;
		}

		#region Old Api Calls
		private static void execute_CurrentModelRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<CurrentModelRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<CurrentModelRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_CurrentModelRequest.Execute(apibaseMessage);
		}

		private static void execute_APIStateRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<APIStateRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<APIStateRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_APIStateRequest.Execute(apibaseMessage);
		}

		private static void execute_StatisticsRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<StatisticsRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<StatisticsRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_StatisticsRequest.Execute(apibaseMessage);
		}

		private static void execute_AuthenticationTokenRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<AuthenticationTokenRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<AuthenticationTokenRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_AuthenticationTokenRequest.Execute(apibaseMessage);
		}

		private static void execute_AuthenticationRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<AuthenticationRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<AuthenticationRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_AuthenticationRequest.Execute(apibaseMessage);
		}

		private static void execute_ModelLoadRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ModelLoadRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ModelLoadRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ModelLoadRequest.Execute(apibaseMessage);
		}

		private static void execute_AvailableModelsRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<AvailableModelsRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<AvailableModelsRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_AvailableModelsRequest.Execute(apibaseMessage);
		}

		private static void execute_HotkeysInCurrentModelRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<HotkeysInCurrentModelRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<HotkeysInCurrentModelRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_HotkeysInCurrentModelRequest.Execute(apibaseMessage);
		}

		private static void execute_HotkeyTriggerRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<HotkeyTriggerRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<HotkeyTriggerRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_HotkeyTriggerRequest.Execute(apibaseMessage);
		}

		private static void execute_Live2DParameterListRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<Live2DParameterListRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<Live2DParameterListRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_Live2DParameterListRequest.Execute(apibaseMessage);
		}

		private static void execute_InputParameterListRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<InputParameterListRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<InputParameterListRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_InputParameterListRequest.Execute(apibaseMessage);
		}

		private static void execute_VTSFolderInfoRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<VTSFolderInfoRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<VTSFolderInfoRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_VTSFolderInfoRequest.Execute(apibaseMessage);
		}

		private static void execute_ColorTintRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ColorTintRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ColorTintRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ColorTintRequest.Execute(apibaseMessage);
		}

		private static void execute_ArtMeshListRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ArtMeshListRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ArtMeshListRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ArtMeshListRequest.Execute(apibaseMessage);
		}

		private static void execute_MoveModelRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<MoveModelRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<MoveModelRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_MoveModelRequest.Execute(apibaseMessage);
		}

		private static void execute_ParameterCreationRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ParameterCreationRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ParameterCreationRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ParameterCreationRequest.Execute(apibaseMessage);
		}

		private static void execute_ParameterDeletionRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ParameterDeletionRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ParameterDeletionRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ParameterDeletionRequest.Execute(apibaseMessage);
		}

		private static void execute_InjectParameterDataRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<InjectParameterDataRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<InjectParameterDataRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_InjectParameterDataRequest.Execute(apibaseMessage);
		}

		private static void execute_ParameterValueRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ParameterValueRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ParameterValueRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ParameterValueRequest.Execute(apibaseMessage);
		}

		private static void execute_FaceFoundRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<FaceFoundRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<FaceFoundRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_FaceFoundRequest.Execute(apibaseMessage);
		}

		private static void execute_SceneColorOverlayInfoRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<SceneColorOverlayInfoRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<SceneColorOverlayInfoRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_SceneColorOverlayInfoRequest.Execute(apibaseMessage);
		}

		private static void execute_NDIConfigRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<NDIConfigRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<NDIConfigRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_NDIConfigRequest.Execute(apibaseMessage);
		}

		private static void execute_ExpressionStateRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ExpressionStateRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ExpressionStateRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ExpressionStateRequest.Execute(apibaseMessage);
		}

		private static void execute_ExpressionActivationRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ExpressionActivationRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ExpressionActivationRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ExpressionActivationRequest.Execute(apibaseMessage);
		}

		private static void execute_GetCurrentModelPhysicsRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<GetCurrentModelPhysicsRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<GetCurrentModelPhysicsRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_GetCurrentModelPhysicsRequest.Execute(apibaseMessage);
		}

		private static void execute_SetCurrentModelPhysicsRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<SetCurrentModelPhysicsRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<SetCurrentModelPhysicsRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_SetCurrentModelPhysicsRequest.Execute(apibaseMessage);
		}

		private static void execute_ItemListRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ItemListRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ItemListRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ItemListRequest.Execute(apibaseMessage);
		}

		private static void execute_ItemLoadRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ItemLoadRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ItemLoadRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ItemLoadRequest.Execute(apibaseMessage);
		}

		private static void execute_ItemUnloadRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ItemUnloadRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ItemUnloadRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;

			Plugin.LogMessage($"Processing: {data}");
			Plugin.LogMessage($"Processing type: {apibaseMessage.messageType}");
			Plugin.LogMessage($"Processing file null?: {apibaseMessage.data == null}");
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ItemUnloadRequest.Execute(apibaseMessage);

		}

		private static void execute_ItemAnimationControlRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ItemAnimationControlRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ItemAnimationControlRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ItemAnimationControlRequest.Execute(apibaseMessage);
		}

		private static void execute_ItemMoveRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ItemMoveRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ItemMoveRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ItemMoveRequest.Execute(apibaseMessage);
		}

		private static void execute_EventSubscriptionRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<EventSubscriptionRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<EventSubscriptionRequest>>(data);
			apibaseMessage.data.message = data;
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_EventSubscriptionRequest.Execute(apibaseMessage);
		}

		private static void execute_ArtMeshSelectionRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ArtMeshSelectionRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ArtMeshSelectionRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ArtMeshSelectionRequest.Execute(apibaseMessage);
		}

		private static void execute_ItemPinRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ItemPinRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ItemPinRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_ItemPinRequest.Execute(apibaseMessage);
		}

		private static void execute_PermissionRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<PermissionRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<PermissionRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_PermissionRequest.Execute(apibaseMessage);
		}

		private static void execute_LoadModelFromURLRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<LoadModelFromURLRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<LoadModelFromURLRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_LoadModelFromURLRequest.Execute(apibaseMessage);
		}

		private static void execute_PostProcessingListRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<PostProcessingListRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<PostProcessingListRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_PostProcessingListRequest.Execute(apibaseMessage);
		}

		private static void execute_PostProcessingUpdateRequest(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<PostProcessingUpdateRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<PostProcessingUpdateRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;
			VTubeStudioAPI_Detour.normalExecutor.ExecutorInstance_PostProcessingUpdateRequest.Execute(apibaseMessage);
		}
		#endregion

		private static void execute_ExtendedDropImage(string sessionID, string requestID, string data, AuthenticatedSession auth)
		{
			APIBaseMessage<ExtendedDropItemRequest> apibaseMessage = JsonUtility.FromJson<APIBaseMessage<ExtendedDropItemRequest>>(data);
			apibaseMessage.sessionAuthInfo = auth;
			apibaseMessage.websocketSessionID = sessionID;
			apibaseMessage.requestID = requestID;

			if (apibaseMessage.data == null)
			{
				var temp = JObject.Parse(data);
				if (temp != null && temp["data"] != null)
				{
					apibaseMessage.data = JsonUtility.FromJson<ExtendedDropItemRequest>(temp["data"].ToString());
				}
			}

			VTubeStudioAPI_Detour.extendedExecutor.ExecutorInstance_ExtendedDropImageRequest.Execute(apibaseMessage);
		}

		/// <summary>
		/// This is because the JsonUtility from Unity fails to convert data node.
		/// </summary>
		/// <typeparam name="T">IAPIMessage message</typeparam>
		/// <param name="responseToSend">Respond to send</param>
		public static async void sendToSession<T>(APIBaseMessage<T> responseToSend) where T : IAPIMessage
		{
			var text = JsonConvert.SerializeObject(responseToSend);

			await Task.Run(delegate
			{
				sendToSession(responseToSend.websocketSessionID, text, sendAsync: false);
			});
		}

		private static void sendToSession(string sessionID, string responseToSend, bool sendAsync)
		{
			Debug.LogError("Sending response session ID: " + sessionID);
			Debug.LogError("Sending response: " + responseToSend);
			bool logDebugResponse = false;
			if (VTubeStudioAPI.doAPILog_DEBUG)
			{
				VTubeStudioAPI.APIDebug("[API][A->C] " + responseToSend, false);
			}
			if (sessionID.IsNullOrEmptyOrWhitespace())
			{
				VTubeStudioAPI.APIDebug("Failed to send API response: Empty or unknown session ID.", false);
				return;
			}
			WebSocket webSocket;
			if (!VTubeStudioAPI_Detour.sessions.TryGetValue(sessionID, out webSocket) || webSocket == null || !webSocket.IsAlive)
			{
				VTubeStudioAPI.APIDebug("Error while attempting to send message to session ID " + sessionID + ". Maybe the socket is already closed?", false);
				return;
			}
			if (sendAsync)
			{
				webSocket.SendAsync(responseToSend, delegate (bool success)
				{
					if (!success)
					{
						VTubeStudioAPI.APIDebug("Failed to send message to session ID " + sessionID + ". Maybe the socket is already closed?", false);
						return;
					}
					if (logDebugResponse)
					{
						VTubeStudioAPI.APIDebug("Sent response to " + sessionID + ": " + responseToSend, false);
					}
				});
				return;
			}
			if (logDebugResponse)
			{
				VTubeStudioAPI.APIDebug("Sending response to " + sessionID + ": " + responseToSend, false);
			}
			webSocket.Send(responseToSend);
		}
	}
}
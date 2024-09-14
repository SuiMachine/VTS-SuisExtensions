using Newtonsoft.Json;
using SuisApiExtension.API;
using UnityEngine;

public abstract class IAPIRequestCustomExecutor : MonoBehaviour
{
	private bool Initialize()
	{
		this.API = base.transform.parent.parent.GetComponent<VTubeStudioAPI>();
		return this.API != null;
	}

	public int GetNumberOfRequests()
	{
		return this.numberReceivedRequests;
	}

	protected abstract void ExecuteInternal(APICustomMessage payload);

	protected abstract bool InitializeInternal();

	protected abstract string GetExecutorRequestName();

	internal void ProcessMessage(string sessionID, string requestID, string data, AuthenticatedSession auth)
	{
		APICustomMessage customMessage = JsonConvert.DeserializeObject<APICustomMessage>(data);

		customMessage.sessionAuthInfo = auth;
		customMessage.websocketSessionID = sessionID;
		customMessage.requestID = requestID;
		Execute(customMessage);
	}

	public void Execute(APICustomMessage payload)
	{
		this.numberReceivedRequests++;
		this.lastRequestAuth = payload.sessionAuthInfo;
		if (!this.initialized)
		{
			if (!this.Initialize() || !this.InitializeInternal())
			{
				Debug.LogError("[VTubeCustomStudioAPI] Received request of type \"" + this.GetExecutorRequestName() + "\" but failed to process it because the responsible request executor couldn't be initialized.");
				this.initialized = false;
				if (this.API != null)
				{
					Debug.LogError("[VTubeCustomStudioAPI] Cannot return error response on API because VTube Studio API failed to initialize.");

					//Call below isn't replicated... also generic again
					//this.API.SendInternalError<T>(payload);
				}
				return;
			}
			this.initialized = true;
			Debug.Log("[VTubeCustomStudioAPI] Initialized API request executor for request type \"" + this.GetExecutorRequestName() + "\".");
		}
		this.ExecuteInternal(payload);
	}

	[SerializeField] protected bool initialized;
	[SerializeField] protected int numberReceivedRequests;
	[SerializeField] protected AuthenticatedSession lastRequestAuth;
	protected VTubeStudioAPI API;
}
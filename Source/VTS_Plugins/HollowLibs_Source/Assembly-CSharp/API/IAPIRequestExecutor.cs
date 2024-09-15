using UnityEngine;

public abstract class IAPIRequestExecutor<T> : MonoBehaviour where T : IAPIMessage
{
	// Token: 0x0600033C RID: 828 RVA: 0x000157E1 File Offset: 0x000139E1
	private bool Initialize()
	{
		this.API = base.transform.parent.parent.GetComponent<VTubeStudioAPI>();
		return this.API != null;
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0001580C File Offset: 0x00013A0C
	public void Execute(APIBaseMessage<T> payload)
	{
		this.numberReceivedRequests++;
		this.lastRequestAuth = payload.sessionAuthInfo;
		if (!this.initialized)
		{
			if (!this.Initialize() || !this.InitializeInternal())
			{
				Debug.LogError("[VTubeStudioAPI] Received request of type \"" + this.GetExecutorRequestName() + "\" but failed to process it because the responsible request executor couldn't be initialized.");
				this.initialized = false;
				if (this.API != null)
				{
					Debug.LogError("[VTubeStudioAPI] Cannot return error response on API because VTube Studio API failed to initialize.");
					this.API.SendInternalError<T>(payload);
				}
				return;
			}
			this.initialized = true;
			Debug.Log("[VTubeStudioAPI] Initialized API request executor for request type \"" + this.GetExecutorRequestName() + "\".");
		}
		this.ExecuteInternal(payload);
	}

	public int GetNumberOfRequests()
	{
		return this.numberReceivedRequests;
	}

	protected abstract void ExecuteInternal(APIBaseMessage<T> payload);

	protected abstract bool InitializeInternal();

	protected abstract string GetExecutorRequestName();

	[SerializeField] protected bool initialized;

	[SerializeField] protected int numberReceivedRequests;

	[SerializeField] protected AuthenticatedSession lastRequestAuth;

	protected VTubeStudioAPI API;
}
using System;
using UnityEngine;

[Serializable]
public class APIBaseMessage<T> where T : IAPIMessage
{
	[NonSerialized] public string websocketSessionID;
	[NonSerialized] public AuthenticatedSession sessionAuthInfo;

	public string apiName;
	public string apiVersion;
	public long timestamp;
	public string messageType;
	public string requestID;
	[SerializeField] public T data;
}

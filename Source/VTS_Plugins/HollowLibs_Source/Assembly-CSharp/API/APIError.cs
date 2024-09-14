using System;

[Serializable]
public class APIError : IAPIMessage
{
	[NonSerialized] public const string NAME = "APIError";
	public ErrorID errorID;
	public string message;
}

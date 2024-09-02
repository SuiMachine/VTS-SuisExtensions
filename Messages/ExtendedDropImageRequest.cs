using System;

[Serializable]
public class ExtendedDropItemRequest : IAPIMessage
{
	[NonSerialized] public const string NAME = nameof(ExtendedDropItemRequest);

	public string fileName = "";
	public float scale = 1.0f;
}

[Serializable]
public class ExtendedDropItemResponse : IAPIMessage
{
	[NonSerialized]
	public const string NAME = nameof(ExtendedDropItemResponse);
	public bool success;
}

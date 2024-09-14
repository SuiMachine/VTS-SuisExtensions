using System;

namespace Assets.ExtendedDropImages.Messages
{
	[Serializable]
	public class ExtendedDropItemRequest : IAPIMessage
	{
		[NonSerialized] public const string NAME = nameof(ExtendedDropItemRequest);

		public string fileName = "";
		public int count = 1;
	}

	[Serializable]
	public class ExtendedDropItemResponse : IAPIMessage
	{
		[NonSerialized]
		public const string NAME = nameof(ExtendedDropItemResponse);
		public bool success;
	}
}

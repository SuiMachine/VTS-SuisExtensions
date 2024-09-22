using System;

namespace Assets.ExtendedDropImages.Messages
{
	[Serializable]
	public class ExtendedDropItemRequest : IAPIMessage
	{
		[NonSerialized] public const string NAME = nameof(ExtendedDropItemRequest);

		public string fileName = "";
		public int count = 1;
		public ExtendedDropItemDefinition dropDefinition;
	}

	[Serializable]
	public class ExtendedDropItemDefinition
	{
		[NonSerialized] public const string NAME = nameof(ExtendedDropItemDefinition);

		public bool normalizeScale = false;
		public bool startWithSmoothBorder = true;
		public float lifeTime = 3.0f;
		public float opacity = 1.0f;
		public float animationSpeed = 1.0f;
		public float gravity = 1.0f;
		public float sizeScale = 1f;
		public float dropSpeed = 0.6f;
		public float bounciness = 0.4f;
		public float rotation = 1.0f;
		public int bottomEdgeBounce = 2;
		public int topEdgeBounce = 2;
		public int leftEdgeBounce = 2;
		public int rightEdgeBounce = 2;
	}

	[Serializable]
	public class ExtendedDropItemResponse : IAPIMessage
	{
		[NonSerialized]
		public const string NAME = nameof(ExtendedDropItemResponse);
		public bool success;
	}
}

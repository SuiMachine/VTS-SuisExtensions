using Assets.ExtendedDropImages.Messages;
using UnityEngine;

public class ExtendedImagesDropper : IAPIRequestExecutor<ExtendedDropItemRequest>
{
	public static ExtendedImagesDropper Instance;
	[SerializeField] private ExtendedDroppedImageBehaviour spawnObjectReference;

	void Awake()
	{
		if(Instance != null)
		{
			Destroy(this.gameObject);
		}
		else
			Instance = this;
	}

	private void OnDestroy()
	{

	}


#if UNITY_EDITOR
	[EasyButtons.Button]
#endif
	public void Spawn()
	{
		var newObject = GameObject.Instantiate(spawnObjectReference);
		newObject.gameObject.SetActive(true);
	}

	protected override void ExecuteInternal(APIBaseMessage<ExtendedDropItemRequest> payload)
	{
		Debug.LogError("Kurwa dziala?");
	}

	protected override bool InitializeInternal() => true;

	protected override string GetExecutorRequestName() => nameof(ExtendedDropItemRequest);
}

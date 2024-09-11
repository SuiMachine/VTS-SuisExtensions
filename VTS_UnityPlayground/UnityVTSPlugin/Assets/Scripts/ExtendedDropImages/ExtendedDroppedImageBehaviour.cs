using UnityEngine;

public class ExtendedDroppedImageBehaviour : MonoBehaviour
{
    [SerializeField] private int kurwaTest = 0;
	[SerializeField] private SpriteRenderer spriteRenderer;

	// Start is called before the first frame update
	void Start()
    {
        
    }

	public void SetItemOrder(int newOrderForItem)
	{
		//this.ItemInfo.Order = newOrderForItem;
		this.spriteRenderer.sortingOrder = (int)(Mathf.Sign((float)newOrderForItem) * 10000f + (float)(newOrderForItem * 650));
		/*		CubismRenderController componentInChildren = this.baseTransform.GetComponentInChildren<CubismRenderController>();
				if (componentInChildren != null)
				{
					this.cubismRenderController = componentInChildren;
					componentInChildren.SortingMode = CubismSortingMode.BackToFrontOrder;
					componentInChildren.SortingOrder = this.itemSpriteRenderer.sortingOrder;
				}*/
	}

	void Update()
    {
        
    }
}

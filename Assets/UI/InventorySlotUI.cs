using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlotUI : Button
{
    private Item renderedItem;

    public InventorySlotUI()
    {
        this.AddToClassList("grid-button");
        this.clicked += () => Debug.Log("Click button");
    }

    public void Update(Item item)
    {
        if (item != renderedItem)
        {
            if (item != null)
            {
                this.style.backgroundImage = new StyleBackground(Prefabs.GetResourceIcon(item.Type));
            }
            else
            {
                this.style.backgroundImage = null;
            }
        }
        
        this.renderedItem = item;
    }
}
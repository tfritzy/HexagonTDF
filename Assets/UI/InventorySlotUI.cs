using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlotUI : Button
{
    private Item renderedItem;
    private ItemType? renderedReservedFor;
    private const string DISABLED_ICON = "disabled-item-background";

    public InventorySlotUI()
    {
        this.AddToClassList("grid-button");
        this.clicked += () => Debug.Log("Click button");
    }

    public void Update(InventoryCell.Slot slot)
    {
        if (slot.Item != renderedItem || this.renderedReservedFor != slot.ReservedFor)
        {
            if (slot.Item != null)
            {
                this.style.backgroundImage = new StyleBackground(Prefabs.GetResourceIcon(slot.Item.Type));
                this.RemoveFromClassList(DISABLED_ICON);
            }
            else
            {
                if (slot.ReservedFor != null)
                {
                    this.style.backgroundImage = new StyleBackground(Prefabs.GetResourceIcon(slot.ReservedFor.Value));
                    this.AddToClassList(DISABLED_ICON);
                }
                else
                {
                    this.style.backgroundImage = null;
                }
            }
        }

        this.renderedReservedFor = slot.ReservedFor;
        this.renderedItem = slot.Item;
    }
}
using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlotUI : Button
{
    private Item renderedItem;
    private ItemType? renderedReservedFor;
    private int? renderedQuantity;
    private const string DISABLED_ICON = "disabled-item-background";

    public InventorySlotUI()
    {
        this.AddToClassList("grid-button");
        this.clicked += () => Debug.Log("Click button");
    }

    public void Update(InventoryCell.Slot slot)
    {
        if (slot.Item != renderedItem ||
            this.renderedReservedFor != slot.ReservedFor ||
            slot.Item?.Quantity != this.renderedQuantity)
        {
            if (slot.Item != null)
            {
                this.text = slot.Item.Quantity > 1 ? slot.Item.Quantity.ToString() : "";
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
        this.renderedQuantity = slot.Item?.Quantity;
    }
}
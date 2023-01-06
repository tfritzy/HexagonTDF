using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlotUI : Button
{
    public int Index { get; private set; }

    private Item renderedItem;
    private ItemType? renderedReservedFor;
    private int? renderedQuantity;
    private Color tintColor = ColorExtensions.Create("#ECBCB3");
    private Color emptyColor = ColorExtensions.Create("#ECBCB3", 128);
    private InventoryUI Parent;

    public InventorySlotUI(InventoryUI parent, int index)
    {
        this.Index = index;
        this.Parent = parent;

        this.AddToClassList("grid-button");
        this.clicked += () => Debug.Log("Click button");
        this.style.backgroundColor = UIColors.Dark.InventorySlotBackground;
        this.SetBorderColor(UIColors.Dark.InventorySlotBackground);

        this.clickable.activators.Clear();
        this.RegisterCallback<MouseDownEvent>(evt =>
        {
            Debug.Log("Pickup item");
            this.Parent.OnSlotMouseDown(this);
        });
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
                this.style.unityBackgroundImageTintColor = new StyleColor(tintColor);
            }
            else
            {
                if (slot.ReservedFor != null)
                {
                    this.style.backgroundImage = new StyleBackground(Prefabs.GetResourceIcon(slot.ReservedFor.Value));
                    this.style.unityBackgroundImageTintColor = new StyleColor(emptyColor);
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
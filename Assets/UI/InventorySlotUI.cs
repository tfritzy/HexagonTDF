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
    private ItemUI itemUI;

    public InventorySlotUI(InventoryUI parent, int index)
    {
        this.Index = index;
        this.Parent = parent;

        this.AddToClassList("grid-button");
        this.clicked += () => Debug.Log("Click button");
        this.style.backgroundColor = UIColors.Dark.InventorySlotBackground;
        this.SetBorderColor(UIColors.Dark.InventorySlotBackground);
        this.style.unityBackgroundImageTintColor = UIColors.Dark.InventorySlotImageTint;
        this.itemUI = new ItemUI();
        this.Add(this.itemUI);

        this.clickable.activators.Clear();
        this.RegisterCallback<MouseDownEvent>(evt =>
        {
            this.Parent?.OnSlotMouseDown(this);
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
                this.itemUI.Update(slot.Item.Type, slot.Item.Quantity, false);
            }
            else
            {
                if (slot.ReservedFor != null)
                {
                    this.itemUI.Update(slot.Item.Type, 0, true);
                }
                else
                {
                    this.itemUI.Blank();
                }
            }
        }

        this.renderedReservedFor = slot.ReservedFor;
        this.renderedItem = slot.Item;
        this.renderedQuantity = slot.Item?.Quantity;
    }

    public void SetDim(bool isDim)
    {
        if (isDim)
        {
            this.style.unityBackgroundImageTintColor = UIColors.Dark.InventorySlotImageTint.Dim(.5f);
        }
        else
        {
            this.style.unityBackgroundImageTintColor = UIColors.Dark.InventorySlotImageTint;
        }
    }
}
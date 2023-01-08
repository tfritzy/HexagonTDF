using UnityEngine;
using UnityEngine.UIElements;

public class ItemUI : VisualElement
{
    private Label quantityLabel;

    public ItemUI()
    {
        this.quantityLabel = new Label();
        this.quantityLabel.AddToClassList("p");
        this.quantityLabel.AddToClassList("outlined");
        this.quantityLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        this.quantityLabel.style.position = Position.Absolute;
        this.quantityLabel.style.bottom = -15;
        this.quantityLabel.style.right = -15;
        this.quantityLabel.pickingMode = PickingMode.Ignore;
        this.pickingMode = PickingMode.Ignore;
        this.Add(this.quantityLabel);
        this.AddToClassList("item");
    }

    public void Update(ItemType itemType, int quantity, bool dim)
    {
        if (quantity == 0)
        {
            Blank();
            return;
        }

        this.quantityLabel.text = quantity > 1 ? quantity.ToString() : "";
        this.style.backgroundImage = new StyleBackground(Prefabs.GetResourceIcon(itemType));

        Color color = UIColors.Dark.InventorySlotImageTint;
        if (dim)
        {
            color = color.Dim(.5f);
        }
        this.style.unityBackgroundImageTintColor = color;
    }

    public void Blank()
    {
        this.style.backgroundImage = null;
        this.style.unityBackgroundImageTintColor = UIColors.Dark.InventorySlotImageTint;
        this.quantityLabel.text = "";
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
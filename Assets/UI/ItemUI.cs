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
        this.quantityLabel.style.bottom = -10;
        this.quantityLabel.style.right = -10;
        this.Add(this.quantityLabel);
        this.AddToClassList("item");
    }

    public void Update(ItemType itemType, int quantity, bool dim)
    {
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
}
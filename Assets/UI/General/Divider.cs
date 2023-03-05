using UnityEngine.UIElements;

public class Divider : VisualElement
{
    public Divider()
    {
        this.AddToClassList("verticalDivider");
        this.SetBorderColor(UIColors.Dark.InventorySlotOutline);
    }
}
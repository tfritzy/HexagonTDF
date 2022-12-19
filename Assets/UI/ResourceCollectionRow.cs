using UnityEngine.UIElements;

public class ResourceCollectionRow : VisualElement
{
    private Label amountResource;
    private VisualElement icon;

    public ResourceCollectionRow()
    {
        this.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

        amountResource = new Label();
        amountResource.AddToClassList("small-outlined-text");
        this.Add(amountResource);

        icon = new VisualElement();
        icon.AddToClassList("resource-collection-hoverer--inline-icon");
        this.Add(icon);
    }

    public void Setup(ItemType itemType, float collectionRate)
    {
        amountResource.text = collectionRate.ToString("n2") + "s /";
        icon.style.backgroundImage = new StyleBackground(Prefabs.GetResourceIcon(itemType));
    }
}
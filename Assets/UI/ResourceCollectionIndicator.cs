using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceCollectionIndicator : UIHoverer
{
    public override Hoverer Type => Hoverer.ResourceCollectionIndicator;
    public override Vector2 Offset => _offset;
    private Vector2 _offset = new Vector2(.5f, -.5f);
    private VisualElement verticalGroup;
    private List<VisualElement> horizontalGroups = new List<VisualElement>();

    public ResourceCollectionIndicator()
    {
        verticalGroup = new VisualElement();
        this.Add(verticalGroup);
    }

    private VisualElement MakeResourceRow(ItemType itemType, float collectionRate)
    {
        VisualElement horizontalGroup = new VisualElement();
        horizontalGroup.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

        Label amountResource = new Label();
        amountResource.text = collectionRate.ToString("n2");
        amountResource.AddToClassList("small-outlined-text");
        horizontalGroup.Add(amountResource);

        VisualElement icon = new VisualElement();
        icon.style.backgroundImage = new StyleBackground(Prefabs.GetResourceIcon(itemType));
        icon.AddToClassList("resource-collection-hoverer--inline-icon");
        horizontalGroup.Add(icon);

        Label perSecond = new Label();
        perSecond.text = "/s";
        perSecond.AddToClassList("small-outlined-text");
        horizontalGroup.Add(perSecond);

        return horizontalGroup;
    }

    public void Init(Dictionary<ItemType, float> collectionRates)
    {
        foreach (ItemType itemType in collectionRates.Keys)
        {
            verticalGroup.Add(MakeResourceRow(itemType, collectionRates[itemType]));
        }
    }
}
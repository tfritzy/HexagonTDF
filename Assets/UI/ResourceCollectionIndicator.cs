using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceCollectionIndicator : UIHoverer
{
    public override Hoverer Type => Hoverer.ResourceCollectionIndicator;
    public override Vector2 Offset => _offset;
    private Vector2 _offset = new Vector2(.5f, -.5f);
    private VisualElement verticalGroup;
    private List<ResourceCollectionRow> horizontalGroups = new List<ResourceCollectionRow>();

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
        amountResource.text = collectionRate.ToString("n2") + "s /";
        amountResource.AddToClassList("small-outlined-text");
        horizontalGroup.Add(amountResource);

        VisualElement icon = new VisualElement();
        icon.style.backgroundImage = new StyleBackground(Prefabs.GetResourceIcon(itemType));
        icon.AddToClassList("resource-collection-hoverer--inline-icon");
        horizontalGroup.Add(icon);

        return horizontalGroup;
    }

    public void Init(Dictionary<ItemType, float> collectionRates)
    {
        int i = 0;
        foreach (ItemType itemType in collectionRates.Keys)
        {
            if (horizontalGroups.Count <= i)
            {
                var row = new ResourceCollectionRow();
                horizontalGroups.Add(row);
                verticalGroup.Add(row);
            }

            horizontalGroups[i].Setup(itemType, collectionRates[itemType]);
            i += 1;
        }
    }
}
using System.Collections.Generic;
using UnityEngine.UIElements;
using static InventoryCell;

public class ConstructionRequirements : VisualElement
{
    private Building renderedBuilding;
    private float renderedConstructionPercent;
    private List<ItemUI> Items;
    private VisualElement row;

    public ConstructionRequirements()
    {
        this.AddToClassList("section");
        this.style.alignItems = Align.Center;
        this.style.width = new Length(100, LengthUnit.Percent);

        Label title = new Label();
        title.text = "Remaining items needed";
        title.AddToClassList("h2");
        this.Add(title);

        this.row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        this.Items = new List<ItemUI>();
        this.Add(row);
    }

    public void Update(Building building)
    {
        if (building != this.renderedBuilding ||
            building.ConstructionPercent != renderedConstructionPercent)
        {
            this.renderedBuilding = building;
            this.renderedConstructionPercent = building.ConstructionPercent;

            while (Items.Count < building.ItemsNeededForConstruction.Keys.Count)
            {
                ItemUI item = new ItemUI();
                Items.Add(item);
                row.Add(item);
            }

            int i = 0;
            foreach (ItemType itemType in building.ItemsNeededForConstruction.Keys)
            {
                Items[i].Update(itemType, building.GetRemainingItemsNeeded(itemType), false);
                i += 1;
            }

            for (; i < Items.Count; i++)
            {
                Items[i].Blank();
            }
        }
    }
}
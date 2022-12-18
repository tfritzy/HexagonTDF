using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : VisualElement
{
    private Label NameLabel;
    private VisualElement itemsContainer;
    private List<InventorySlotUI> itemSlots;

    public InventoryUI()
    {
        VisualElement verticalGroup = new VisualElement();
        this.Add(verticalGroup);

        this.NameLabel = new Label();
        this.NameLabel.AddToClassList("heading-2");
        this.NameLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        verticalGroup.Add(NameLabel);

        itemsContainer = new VisualElement();
        itemsContainer.AddToClassList("grid");
        verticalGroup.Add(itemsContainer);

        itemSlots = new List<InventorySlotUI>();
    }

    public void Update(InventoryCell inventoryCell)
    {
        this.NameLabel.text = inventoryCell.Name;

        while (itemSlots.Count < inventoryCell.Size)
        {
            InventorySlotUI slot = new InventorySlotUI();
            itemSlots.Add(slot);
            itemsContainer.Add(slot);
        }

        int i;
        for (i = 0; i < inventoryCell.Size; i++)
        {
            itemSlots[i].Update(inventoryCell.SlotAt(i));
            itemSlots[i].style.display = DisplayStyle.Flex;
        }

        while (i < itemSlots.Count)
        {
            itemSlots[i].style.display = DisplayStyle.None;
            i += 1;
        }
    }
}
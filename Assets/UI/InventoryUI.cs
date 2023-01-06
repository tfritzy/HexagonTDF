using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : VisualElement
{
    private Label NameLabel;
    private VisualElement itemsContainer;
    private List<InventorySlotUI> itemSlots;
    private InventoryCell inventoryCell;
    private InventoryTransferUI Parent;

    public InventoryUI(InventoryTransferUI parent)
    {
        this.Parent = parent;

        VisualElement verticalGroup = new VisualElement();
        this.Add(verticalGroup);

        this.NameLabel = new Label();
        this.NameLabel.AddToClassList("heading-2");
        this.NameLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        verticalGroup.Add(NameLabel);

        itemsContainer = new VisualElement();
        itemsContainer.style.flexDirection = FlexDirection.Row;
        itemsContainer.style.flexWrap = Wrap.Wrap;
        verticalGroup.Add(itemsContainer);

        itemSlots = new List<InventorySlotUI>();
    }

    public void Update(InventoryCell inventoryCell)
    {
        this.inventoryCell = inventoryCell;
        this.NameLabel.text = inventoryCell.Name;

        while (itemSlots.Count < inventoryCell.Size)
        {
            InventorySlotUI slot = new InventorySlotUI(this, itemSlots.Count);
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

    public void OnSlotMouseDown(InventorySlotUI slot)
    {
        this.Parent.OnSlotMouseDown(this.inventoryCell, slot);
    }
}
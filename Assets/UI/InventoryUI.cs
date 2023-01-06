using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : VisualElement
{
    private Label NameLabel;
    private VisualElement itemsContainer;
    private List<InventorySlotUI> itemSlots;
    private int pickedUpItemIndex;
    private InventoryCell inventoryCell;
    private VisualElement hoveredItem;
    private Vector2 HoverItemOffset = new Vector2(50, 50);

    public InventoryUI()
    {
        this.RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent);
        this.pickedUpItemIndex = -1;

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
        if (this.pickedUpItemIndex == -1)
        {
            if (this.inventoryCell.ItemAt(slot.Index) != null)
            {
                this.pickedUpItemIndex = slot.Index;
                Item item = this.inventoryCell.ItemAt(this.pickedUpItemIndex);

                if (hoveredItem == null)
                {
                    hoveredItem = new VisualElement();
                    hoveredItem.AddToClassList("floating-item");
                    hoveredItem.pickingMode = PickingMode.Ignore;
                }
                else
                {
                    this.hoveredItem.style.display = DisplayStyle.Flex;
                }

                hoveredItem.style.backgroundImage = new StyleBackground(Prefabs.GetResourceIcon(item.Type));
                this.Add(hoveredItem);
            }
        }
        else
        {
            Item item = this.inventoryCell.ItemAt(this.pickedUpItemIndex);
            if (this.inventoryCell.CanAcceptItem(item.Type, slot.Index))
            {
                this.inventoryCell.RemoveAt(this.pickedUpItemIndex);
                this.inventoryCell.AddItem(item, slot.Index);

                this.hoveredItem.style.display = DisplayStyle.None;
                this.pickedUpItemIndex = -1;
            }
        }
    }

    void OnMouseMoveEvent(MouseMoveEvent e)
    {
        if (hoveredItem != null)
        {
            hoveredItem.style.top = e.localMousePosition.y + -HoverItemOffset.y;
            hoveredItem.style.left = e.localMousePosition.x + -HoverItemOffset.x;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryTransferUI : VisualElement
{
    private int? pickedUpItemIndex;
    private InventoryCell pickedUpItemInventory;
    private VisualElement hoveredItem;
    private Vector2 HoverItemOffset = new Vector2(50, 50);
    private List<InventoryUI> inventoriesRendered;
    private VisualElement inventoryContainer;

    public InventoryTransferUI()
    {
        this.RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent);
        this.inventoriesRendered = new List<InventoryUI>();
    }

    public void OnSlotMouseDown(InventoryCell inventory, InventorySlotUI slot)
    {
        if (this.pickedUpItemIndex == null)
        {
            if (inventory.ItemAt(slot.Index) != null)
            {
                this.pickedUpItemInventory = inventory;
                this.pickedUpItemIndex = slot.Index;
                Item item = inventory.ItemAt(this.pickedUpItemIndex.Value);
                slot.SetDim(true);

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
            Item item = this.pickedUpItemInventory.ItemAt(this.pickedUpItemIndex.Value);
            if (inventory.CanAcceptItem(item.Type, slot.Index))
            {
                this.pickedUpItemInventory.RemoveAt(this.pickedUpItemIndex.Value);
                inventory.AddItem(item, slot.Index);

                this.hoveredItem.style.display = DisplayStyle.None;
                this.pickedUpItemIndex = null;
                this.pickedUpItemInventory = null;
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

    private void InitInventories(List<InventoryCell> inventoriesToRender)
    {
        if (this.inventoryContainer == null)
        {
            this.inventoryContainer = new VisualElement();
            this.Add(inventoryContainer);
        }

        int i = 0;
        while (inventoriesRendered.Count < inventoriesToRender.Count)
        {
            InventoryUI inventory = new InventoryUI(this);
            this.inventoriesRendered.Add(inventory);
            this.inventoryContainer.Add(inventory);
            i += 1;
        }
    }

    public void UpdateInventories(List<InventoryCell> inventories)
    {
        InitInventories(inventories);

        int i;
        for (i = 0; i < this.inventoriesRendered.Count; i++)
        {
            this.inventoriesRendered[i].Update(inventories[i]);
            this.inventoriesRendered[i].style.display = DisplayStyle.Flex;
        }

        while (i < this.inventoriesRendered.Count)
        {
            this.inventoriesRendered[i].style.display = DisplayStyle.None;
            i += 1;
        }
    }
}
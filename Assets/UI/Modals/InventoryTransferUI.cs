using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryTransferUI : VisualElement
{
    private InventorySlotUI pickedUpItemSlot;
    private InventoryCell pickedUpItemInventory;
    private ItemUI hoveredItem;
    private Vector2 HoverItemOffset = new Vector2(50, 50);
    private List<InventoryUI> inventoriesRendered;
    private List<InventoryCell> inventories;
    private VisualElement inventoryContainer;
    private bool showTitles;

    public InventoryTransferUI(bool showTitles)
    {
        this.RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent);
        this.inventoriesRendered = new List<InventoryUI>();
        this.showTitles = showTitles;
    }

    public void OnSlotMouseDown(InventoryCell inventory, InventorySlotUI slot)
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            Item item = inventory.ItemAt(slot.Index);
            InventoryCell nextInventory = this.inventories.Find((InventoryCell i) => i != inventory);
            if (nextInventory != null)
            {
                nextInventory.AutomaticTransfer(inventory, slot.Index);
            }
        }
        else if (this.pickedUpItemSlot == null)
        {
            if (inventory.ItemAt(slot.Index) != null)
            {
                this.pickedUpItemInventory = inventory;
                this.pickedUpItemSlot = slot;
                Item item = inventory.ItemAt(this.pickedUpItemSlot.Index);
                slot.SetDim(true);

                if (hoveredItem == null)
                {
                    hoveredItem = new ItemUI();
                    hoveredItem.AddToClassList("floating");
                    this.Add(hoveredItem);
                }
                else
                {
                    this.hoveredItem.style.display = DisplayStyle.Flex;
                }

                hoveredItem.Update(item.Type, item.Quantity, false);
                hoveredItem.style.backgroundImage = new StyleBackground(Prefabs.GetResourceIcon(item.Type));
            }
        }
        else
        {
            Item item = this.pickedUpItemInventory.ItemAt(this.pickedUpItemSlot.Index);
            if (inventory.CanAcceptItem(item.Type, slot.Index))
            {
                // Transfer only 1 item if rmb is held down.
                int quantity = Input.GetMouseButton(1) ? 1 : item.Quantity;

                int remaining = inventory.TransferItem(
                    this.pickedUpItemInventory,
                    this.pickedUpItemSlot.Index,
                    slot.Index,
                    quantity);

                if (this.pickedUpItemInventory.ItemAt(this.pickedUpItemSlot.Index) == null)
                {
                    this.hoveredItem.style.display = DisplayStyle.None;
                    this.pickedUpItemSlot.SetDim(false);
                    this.pickedUpItemSlot = null;
                    this.pickedUpItemInventory = null;
                }
                else
                {
                    this.hoveredItem.Update(item.Type, item.Quantity, false);
                }
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
        this.inventories = inventoriesToRender;

        if (this.inventoryContainer == null)
        {
            this.inventoryContainer = new VisualElement();
            this.Add(inventoryContainer);
        }

        int i = 0;
        while (inventoriesRendered.Count < inventoriesToRender.Count)
        {
            InventoryUI inventory = new InventoryUI(this, showTitles);
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
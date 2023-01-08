using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryCell : Cell
{
    private Slot[] Slots;
    public int Size { get; }
    public string Name { get; private set; }

    public class Slot
    {
        public Item Item;
        public ItemType? ReservedFor;
    };

    public override void Update() { }

    public InventoryCell(int size, string name)
    {
        this.Size = size;
        this.Slots = new Slot[Size];
        this.Name = name;

        for (int i = 0; i < Size; i++)
        {
            this.Slots[i] = new Slot();
        }
    }

    public Slot SlotAt(int index)
    {
        if (index < 0 || index >= Size)
        {
            return null;
        }

        return Slots[index];
    }

    public Item ItemAt(int index)
    {
        if (index < 0 || index >= Size)
        {
            return null;
        }

        return Slots[index].Item;
    }

    public void AddItem(Item item, int index = -1)
    {
        if (index == -1)
        {
            index = GetFirstOpenSlot(item.Type);
        }

        if (index != -1)
        {
            if (Slots[index].Item == null)
            {
                this.Slots[index].Item = item;
            }
            else if (Slots[index].Item.Quantity < Slots[index].Item.MaxStackSize)
            {
                this.Slots[index].Item.Quantity += item.Quantity;
            }
            else
            {
                throw new System.Exception("There's no room.");
            }
        }
        else
        {
            throw new System.Exception("There's no room.");
        }
    }

    public void AutomaticTransfer(InventoryCell fromInventory, int index)
    {
        Item item = fromInventory.ItemAt(index);
        if (item == null)
        {
            return;
        }

        int targetIndex = GetFirstOpenSlot(item.Type);
        while (targetIndex != -1 && fromInventory.ItemAt(index) != null)
        {
            TransferItem(fromInventory, index, targetIndex, item.Quantity);
            targetIndex = GetFirstOpenSlot(item.Type);
        }
    }

    public int TransferItem(InventoryCell fromInventory, int sourceIndex, int targetIndex, int quantity)
    {
        Item item = fromInventory.ItemAt(sourceIndex);
        if (item == null)
        {
            return 0;
        }

        Item targetItem = ItemAt(targetIndex);
        if (targetItem != null)
        {
            int transferQuantity = Math.Min(quantity, targetItem.MaxStackSize - targetItem.Quantity);
            if (transferQuantity > 0)
            {
                int remaining = item.Quantity - transferQuantity;
                fromInventory.RemoveAt(sourceIndex, transferQuantity);
                targetItem.Quantity += transferQuantity;
                return remaining;
            }
            else
            {
                return quantity;
            }
        }
        else
        {
            fromInventory.RemoveAt(sourceIndex, quantity);
            if (fromInventory.ItemAt(sourceIndex) == null)
            {
                AddItem(item, targetIndex);
            }
            else
            {
                Item splitStack = ItemGenerator.Make(item.Type);
                splitStack.Quantity = quantity;
                AddItem(splitStack, targetIndex);
            }

            return 0;
        }
    }

    public void RemoveAt(int index, int quantity = 1)
    {
        if (ItemAt(index) == null)
        {
            return;
        }

        if (quantity >= ItemAt(index).Quantity)
        {
            Slots[index].Item = null;
        }
        else
        {
            ItemAt(index).Quantity -= quantity;
        }
    }

    public bool CanAcceptItem(ItemType itemType)
    {
        // TODO respect quantity.
        return GetFirstOpenSlot(itemType) != -1;
    }

    public bool CanAcceptItem(ItemType itemType, int index)
    {
        if (ItemAt(index) == null)
        {
            return true;
        }

        if (SlotAt(index).ReservedFor != null &&
            SlotAt(index).ReservedFor != itemType)
        {
            return false;
        }

        return ItemAt(index).Quantity < ItemAt(index).MaxStackSize;
    }

    private int GetFirstOpenSlot(ItemType forItem)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].Item == null &&
               (Slots[i].ReservedFor == null || Slots[i].ReservedFor == forItem))
            {
                return i;
            }

            if (Slots[i].Item != null &&
                Slots[i].Item.Type == forItem &&
                Slots[i].Item.Quantity < Slots[i].Item.MaxStackSize)
            {
                return i;
            }
        }

        return -1;
    }

    public int[] FirstIndeciesOf(ItemType[] items)
    {
        int[] indecies = new int[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            indecies[i] = -1;
        }

        for (int i = 0; i < items.Length; i++)
        {
            indecies[i] = FirstIndexOfItem(items[i]);
        }

        return indecies;
    }


    public int FirstNonEmptyIndex()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].Item != null)
            {
                return i;
            }
        }

        return -1;
    }

    public int FirstIndexOfItem(ItemType ofType)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].Item != null && Slots[i].Item.Type == ofType)
            {
                return i;
            }
        }

        return -1;
    }

    public void MakeSlotReserved(int index, ItemType itemType)
    {
        this.Slots[index].ReservedFor = itemType;
    }
}
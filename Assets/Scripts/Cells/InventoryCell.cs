using System.Collections.Generic;
using System.Linq;

public class InventoryCell : Cell
{
    private Slot[] Slots;
    public int Size { get; }
    public string Name { get; private set; }

    public class Slot {
        public Item Item;
        public ItemType? ReservedFor;
    };

    public override void Update() { }

    public InventoryCell(int size, string name)
    {
        this.Size = size;
        this.Slots = new Slot[Size];
        this.Name = name;

        for(int i = 0; i < Size; i++)
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

    public void AddItem(Item item)
    {
        int firstOpenSlot = GetFirstOpenSlot(item.Type);
        if (firstOpenSlot != -1)
        {
            this.Slots[firstOpenSlot].Item = item;
        } else
        {
            throw new System.Exception("There's no room.");
        }
    }

    public void TransferItem(InventoryCell fromInventory, int itemIndex)
    {
        Item itemBeingTransfered = fromInventory.ItemAt(itemIndex);
        int firstOpenSlot = GetFirstOpenSlot(itemBeingTransfered.Type);
        if (firstOpenSlot != -1)
        {
            this.Slots[firstOpenSlot].Item = fromInventory.Slots[itemIndex].Item;
            fromInventory.Slots[itemIndex].Item = null;
        }
    }

    public void RemoveAt(int index)
    {
        Slots[index].Item = null;
    }

    public bool CanAcceptItem(ItemType itemType)
    {
        return GetFirstOpenSlot(itemType) != -1;
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
        }

        return -1;
    }

    public int FirstItemIndex()
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

    public int GetFirstItemIndex(ItemType ofType)
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
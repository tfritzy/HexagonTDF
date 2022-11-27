using System.Collections.Generic;
using System.Linq;

public class InventoryCell : Cell
{
    private Item[] Items;
    public int Size { get; }
    public bool IsFull => GetFirstOpenSlot() == -1;

    public override void Update() { }

    public InventoryCell(int size)
    {
        this.Size = size;
        this.Items = new Item[Size];
    }

    public Item ItemAt(int index)
    {
        if (index < 0 || index >= Size)
        {
            return null;
        }

        return Items[index];
    }

    public void AddItem(Item item)
    {
        int firstOpenSlot = GetFirstOpenSlot();
        if (firstOpenSlot != -1)
        {
            this.Items[firstOpenSlot] = item;
        }
    }

    public void TransferItem(InventoryCell fromInventory, int itemIndex)
    {
        int firstOpenSlot = GetFirstOpenSlot();
        if (firstOpenSlot != -1)
        {
            this.Items[firstOpenSlot] = fromInventory.Items[itemIndex];
            fromInventory.Items[itemIndex] = null;
        }
    }

    public void RemoveAt(int index)
    {
        Items[index] = null;
    }

    private int GetFirstOpenSlot()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == null)
            {
                return i;
            }
        }

        return -1;
    }

    public int FirstItemIndex()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] != null)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetFirstItemIndex(ItemType ofType)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] != null && Items[i].Type == ofType)
            {
                return i;
            }
        }

        return -1;
    }
}
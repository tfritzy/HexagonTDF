using System.Collections.Generic;
using System.Linq;

public class InventoryCell : Cell
{
    private Resource[] Items;
    public int Size { get; }

    public InventoryCell(int size)
    {
        this.Size = size;
    }

    public override void Update() { }

    public InventoryCell()
    {
        this.Items = new Resource[Size];
    }

    public Resource GetItemAt(int index)
    {
        if (index < 0 || index >= Size)
        {
            return null;
        }

        return Items[index];
    }

    public void AddItem(Resource item)
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

    public Resource GetFirstItem(ResourceType ofType)
    {
        foreach (Resource resource in Items)
        {
            if (resource.Type == ofType)
            {
                return resource;
            }
        }

        return null;
    }
}
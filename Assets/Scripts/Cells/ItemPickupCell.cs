using UnityEngine;

public class ItemPickupCell : Cell
{
    // Which items are picked up from the conveyor, or everything if null.
    private ItemType[] acceptedItems;
    private InventoryCell intoInventory;

    public ItemPickupCell(
        ItemType[] itemsToAccept,
        InventoryCell intoInventory)
    {
        this.acceptedItems = itemsToAccept;
        this.intoInventory = intoInventory;
    }

    public override void Update()
    {
        PickupFromConveyor();
    }

    private void PickupFromConveyor()
    {
        foreach (ItemType itemType in this.acceptedItems)
        {
            foreach (ConveyorCell.Belt belt in this.Owner.ConveyorCell.InputBelts.Values)
            {
                var furthestResource = this.Owner.ConveyorCell.GetFurthestAlongResourceOfType(belt, itemType);
                if (furthestResource != null && furthestResource.ProgressAlongPath > .2f)
                {
                    if (furthestResource.ItemInst.Item.Type == itemType && intoInventory.CanAcceptItem(itemType))
                    {
                        Item item = furthestResource.ItemInst.Item;
                        intoInventory.AddItem(item);
                        GameObject.Destroy(furthestResource.ItemInst.gameObject);
                        this.Owner.ConveyorCell.RemoveItem(belt, item.Id);
                    }
                    else
                    {
                        furthestResource.IsPaused = true;
                    }
                }
            }
        }
    }
}
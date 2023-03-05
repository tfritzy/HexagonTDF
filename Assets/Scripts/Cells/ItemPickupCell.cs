using UnityEngine;

public class ItemPickupCell : Cell
{
    // Which items are picked up from the conveyor, or everything if null.
    private ItemType[] acceptedItems;
    private InventoryCell intoInventory;
    private ConveyorCell conveyorCell;

    public ItemPickupCell(
        ConveyorCell conveyorCell,
        ItemType[] itemsToAccept,
        InventoryCell intoInventory)
    {
        this.acceptedItems = itemsToAccept;
        this.intoInventory = intoInventory;
        this.conveyorCell = conveyorCell;
    }

    public override void Update()
    {
        PickupFromConveyor();
    }

    private void PickupFromConveyor()
    {
        if (conveyorCell?.ConveyorBelt == null)
        {
            return;
        }

        foreach (ItemType itemType in this.acceptedItems)
        {
            var furthestResource = conveyorCell.GetFurthestAlongResourceOfType(conveyorCell.ConveyorBelt, itemType);
            if (furthestResource != null && furthestResource.ProgressAlongPath > .2f)
            {
                if (furthestResource.ItemInst.Item.Type == itemType && intoInventory.CanAcceptItem(itemType))
                {
                    Item item = furthestResource.ItemInst.Item;
                    intoInventory.AddItem(item);
                    conveyorCell.RemoveItem(conveyorCell.ConveyorBelt, item.Id);
                    GameObject.Destroy(furthestResource.ItemInst.gameObject);

                    var newFurthest = conveyorCell.GetFurthestAlongResource(conveyorCell.ConveyorBelt);
                    if (newFurthest != null && newFurthest.ProgressAlongPath > .2f)
                    {
                        newFurthest.IsPaused = true;
                    }
                }
                else
                {
                    furthestResource.IsPaused = true;
                }
            }
        }
    }
}
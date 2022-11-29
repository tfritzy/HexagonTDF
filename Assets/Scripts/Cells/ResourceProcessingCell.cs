using UnityEngine;

public abstract class ResourceProcessingCell : Cell
{
    public abstract InventoryCell InputInventory { get; }
    public virtual InventoryCell ProcessingInventory => processingInventory;
    public abstract ItemType OutputItemType { get; }
    public abstract ItemType InputItemType { get; }
    public abstract float SecondsToProcessResource { get; }
    private InventoryCell processingInventory;
    private float? processingStartTime;

    public override void Setup(Character owner)
    {
        processingInventory = new InventoryCell(1);

        base.Setup(owner);
    }

    public override void Update()
    {
        var furthestResource = this.Owner.ConveyorCell.GetFurthestAlongResourceOfType(InputItemType);
        if (furthestResource != null && furthestResource.ProgressAlongPath > .2f)
        {
            if (!InputInventory.IsFull)
            {
                Item item = furthestResource.ItemInst.Item;
                InputInventory.AddItem(item);
                GameObject.Destroy(furthestResource.ItemInst.gameObject);
                this.Owner.ConveyorCell.RemoveItem(item.Id);
            }
            else
            {
                furthestResource.IsPaused = true;
            }
        }

        int firstEligableIndex = InputInventory.GetFirstItemIndex(InputItemType);
        if (firstEligableIndex != -1 && !ProcessingInventory.IsFull)
        {
            ProcessingInventory.TransferItem(InputInventory, firstEligableIndex);
        }

        int firstProcessableIndex = ProcessingInventory.GetFirstItemIndex(InputItemType);
        if (firstProcessableIndex != -1)
        {
            if (processingStartTime.HasValue && Time.time - processingStartTime > SecondsToProcessResource)
            {
                GameObject newResource = GameObject.Instantiate(
                    Prefabs.GetResource(OutputItemType),
                    Vector3.zero,
                    Prefabs.GetResource(OutputItemType).transform.rotation
                );

                Item item = ItemGenerator.Make(OutputItemType);
                InstantiatedItem itemInst = newResource.AddComponent<InstantiatedItem>();
                itemInst.Init(item);
                this.Owner.ConveyorCell.AddItem(itemInst, 1.2f); // TODO: some specific placement.
                this.ProcessingInventory.RemoveAt(firstProcessableIndex);
                processingStartTime = null;
            }
            else if (processingStartTime == null)
            {
                processingStartTime = Time.time;
            }
        }
    }
}
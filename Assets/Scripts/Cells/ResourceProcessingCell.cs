using UnityEngine;

public abstract class ResourceProcessingCell : Cell
{
    public virtual InventoryCell InputInventory => inputInventory;
    public virtual InventoryCell ProcessingInventory => processingInventory;
    public virtual InventoryCell OutputInventory => outputInventory;
    public abstract ItemType OutputItemType { get; }
    public abstract ItemType InputItemType { get; }
    public abstract float SecondsToProcessResource { get; }
    private InventoryCell inputInventory;
    private InventoryCell processingInventory;
    private InventoryCell outputInventory;
    private float? processingStartTime;
    private float itemWidth;
    private const float ITEM_SPAWN_PROGRESS = 1.2f;

    public override void Setup(Character owner)
    {
        inputInventory = new InventoryCell(3, "Input");
        processingInventory = new InventoryCell(1, "Processing");
        outputInventory = new InventoryCell(3, "Output");
        
        this.itemWidth = ItemGenerator.Make(OutputItemType).Width;

        base.Setup(owner);
    }


    float lastUpdateTime;
    public override void Update()
    {
        if (Time.time - lastUpdateTime < .25f)
        {
            return;
        }
        lastUpdateTime = Time.time;

        PickupFromConveyor();
        TransferToProcessing();
        TransferToOutput();
        PlaceOutputOnConveyor();
    }

    private void PickupFromConveyor()
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
    }

    private void TransferToProcessing()
    {
        int firstEligableIndex = InputInventory.GetFirstItemIndex(InputItemType);
        if (firstEligableIndex != -1 && !ProcessingInventory.IsFull)
        {
            ProcessingInventory.TransferItem(InputInventory, firstEligableIndex);
        }
    }

    private void TransferToOutput()
    {
        int firstProcessableIndex = ProcessingInventory.GetFirstItemIndex(InputItemType);
        if (firstProcessableIndex != -1 && !outputInventory.IsFull)
        {
            if (processingStartTime.HasValue && Time.time - processingStartTime > SecondsToProcessResource)
            {
                Item item = ItemGenerator.Make(OutputItemType);
                this.ProcessingInventory.RemoveAt(firstProcessableIndex);
                this.OutputInventory.AddItem(item);
                this.processingStartTime = null;
            }
            else if (processingStartTime == null)
            {
                processingStartTime = Time.time;
            }
        }
    }

    private void PlaceOutputOnConveyor()
    {
        int firstOutputIndex = OutputInventory.FirstItemIndex();
        if (firstOutputIndex != -1)
        {
            if (this.Owner.ConveyorCell.CanAccept(this.itemWidth, ITEM_SPAWN_PROGRESS))
            {
                GameObject newResource = GameObject.Instantiate(
                    Prefabs.GetResource(OutputItemType),
                    Vector3.zero,
                    Prefabs.GetResource(OutputItemType).transform.rotation
                );

                Item item = OutputInventory.ItemAt(firstOutputIndex);
                InstantiatedItem itemInst = newResource.AddComponent<InstantiatedItem>();
                itemInst.Init(item);
                this.Owner.ConveyorCell.AddItem(itemInst, 1.2f); // TODO: some specific placement.
                this.OutputInventory.RemoveAt(firstOutputIndex);
            }
        }
    }
}
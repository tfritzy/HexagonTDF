using UnityEngine;

public abstract class ResourceProcessingCell : Cell
{
    public virtual InventoryCell InputInventory => _inputInventory;
    public virtual InventoryCell ProcessingInventory => _processingInventory;
    public virtual InventoryCell OutputInventory => _outputInventory;
    public abstract ItemType OutputItemType { get; }
    public abstract ItemType InputItemType { get; }
    public abstract float SecondsToProcessResource { get; }
    public abstract float PercentOfInputConsumedPerOutput { get; }
    private InventoryCell _inputInventory;
    private InventoryCell _processingInventory;
    private InventoryCell _outputInventory;
    private float? processingStartTime;
    private float itemWidth;
    private const float ITEM_SPAWN_PROGRESS = 1.2f;

    public override void Setup(Character owner)
    {
        _inputInventory = new InventoryCell(3, "Input");
        _processingInventory = new InventoryCell(1, "Processing");
        _outputInventory = new InventoryCell(3, "Output");
        InputInventory.MakeSlotReserved(0, ItemType.ArrowShaft);

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
        Process();
        PlaceOutputOnConveyor();
    }

    private void PickupFromConveyor()
    {
        var furthestResource = this.Owner.ConveyorCell.GetFurthestAlongResourceOfType(InputItemType);
        if (furthestResource != null && furthestResource.ProgressAlongPath > .2f)
        {
            if (furthestResource.ItemInst.Item.Type == InputItemType && 
                InputInventory.CanAcceptItem(InputItemType))
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
        if (firstEligableIndex != -1 && ProcessingInventory.CanAcceptItem(InputInventory.ItemAt(firstEligableIndex).Type))
        {
            ProcessingInventory.TransferItem(InputInventory, firstEligableIndex);
        }
    }

    private void Process()
    {
        int firstItem = ProcessingInventory.GetFirstItemIndex(InputItemType);
        if (firstItem != -1 && OutputInventory.CanAcceptItem(ProcessingInventory.ItemAt(firstItem).Type))
        {
            if (processingStartTime.HasValue && Time.time - processingStartTime > SecondsToProcessResource)
            {
                Item item = ItemGenerator.Make(OutputItemType);
                this.processingStartTime = null;
                this.OutputInventory.AddItem(item);

                if (ProcessingInventory.ItemAt(firstItem).RemainingPercent >= PercentOfInputConsumedPerOutput)
                {
                    ProcessingInventory.ItemAt(firstItem).RemainingPercent -= PercentOfInputConsumedPerOutput;
                }
                else
                {
                    this.ProcessingInventory.RemoveAt(firstItem);
                }
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
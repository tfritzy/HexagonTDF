using System.Linq;
using UnityEngine;

public abstract class ResourceProcessingCell : Cell
{
    public virtual InventoryCell InputInventory => _inputInventory;
    public virtual InventoryCell ProcessingInventory => _processingInventory;
    public virtual InventoryCell OutputInventory => _outputInventory;
    public abstract ItemType OutputItemType { get; }
    public abstract float SecondsToProcessResource { get; }
    public abstract float PercentOfInputConsumedPerOutput { get; }
    private InventoryCell _inputInventory;
    private InventoryCell _processingInventory;
    private InventoryCell _outputInventory;
    private float? processingStartTime;
    private float itemWidth;
    private ItemType[] inputItems;
    private ItemPickupCell pickupCell;

    public override void Setup(Character owner)
    {
        Item item = ItemGenerator.Make(OutputItemType);
        this.itemWidth = item.Width;
        this.inputItems = item.Ingredients;

        _inputInventory = new InventoryCell(4, "Input");
        _processingInventory = new InventoryCell(inputItems.Length, "Processing");
        _outputInventory = new InventoryCell(4, "Output");

        this.pickupCell = new ItemPickupCell(inputItems, this.InputInventory);
        InitReservedSlots();
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

        this.pickupCell.Update();
        TransferToProcessing();
        Process();
        PlaceOutputOnConveyor();
    }

    private void TransferToProcessing()
    {
        foreach (ItemType item in inputItems)
        {
            int firstEligableIndex = InputInventory.FirstIndexOfItem(item);
            if (firstEligableIndex != -1 && ProcessingInventory.CanAcceptItem(InputInventory.ItemAt(firstEligableIndex).Type))
            {
                ProcessingInventory.TransferItem(InputInventory, firstEligableIndex);
            }
        }
    }

    private void Process()
    {
        int[] firstItems = ProcessingInventory.FirstIndeciesOf(inputItems);
        if (firstItems.All((int index) => index != -1) && OutputInventory.CanAcceptItem(OutputItemType))
        {
            if (processingStartTime.HasValue && Time.time - processingStartTime > SecondsToProcessResource)
            {
                this.processingStartTime = null;

                foreach (int index in firstItems)
                {
                    if (ProcessingInventory.ItemAt(index).RemainingPercent >= PercentOfInputConsumedPerOutput)
                    {
                        ProcessingInventory.ItemAt(index).RemainingPercent -= PercentOfInputConsumedPerOutput;
                    }
                    else
                    {
                        this.ProcessingInventory.RemoveAt(index);
                    }
                }

                Item item = ItemGenerator.Make(OutputItemType);
                this.OutputInventory.AddItem(item);
            }
            else if (processingStartTime == null)
            {
                processingStartTime = Time.time;
            }
        }
    }

    private void PlaceOutputOnConveyor()
    {
        int outputIndex = OutputInventory.FirstNonEmptyIndex();
        if (outputIndex != -1)
        {
            if (this.Owner.ConveyorCell.CanAccept(this.Owner.ConveyorCell.OutputBelt, this.itemWidth))
            {
                GameObject newResource = GameObject.Instantiate(
                    Prefabs.GetResource(OutputItemType),
                    this.Owner.ConveyorCell.OutputBelt.Points[0],
                    Prefabs.GetResource(OutputItemType).transform.rotation
                );

                Item item = OutputInventory.ItemAt(outputIndex);
                InstantiatedItem itemInst = newResource.AddComponent<InstantiatedItem>();
                itemInst.Init(item);
                this.Owner.ConveyorCell.AddItem(this.Owner.ConveyorCell.OutputBelt, itemInst);
                this.OutputInventory.RemoveAt(outputIndex);
            }
        }
    }

    private void InitReservedSlots()
    {
        int reservedPerItem = InputInventory.Size / inputItems.Length;
        int reserveIndex = 0;
        foreach (ItemType item in inputItems)
        {
            for (int j = 0; j < reservedPerItem; j++)
            {
                InputInventory.MakeSlotReserved(reserveIndex, item);
                reserveIndex += 1;
            }
        }

        for (int i = 0; i < inputItems.Length; i++)
        {
            ProcessingInventory.MakeSlotReserved(i, inputItems[i]);
        }
    }
}
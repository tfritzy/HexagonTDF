using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceCollectionCell : Cell
{
    public abstract Dictionary<ItemType, float> BaseSecondsPerResource { get; }
    public Dictionary<ItemType, float> SecondsPerResourceCollection => _secondsPerResource;
    private Dictionary<ItemType, float> _secondsPerResource;
    public abstract Dictionary<Biome, ItemType> BiomesCollectedFrom { get; }
    public virtual int CollectionRange => 1;
    public virtual InventoryCell Inventory => outputInventory;
    private InventoryCell outputInventory;

    public override void Setup(Character character)
    {
        base.Setup(character);
        outputInventory = new InventoryCell(3, "Inventory");
        InitCollectionRates();
    }

    public override void Update()
    {
        HarvestResources();
    }

    private void InitCollectionRates()
    {
        _secondsPerResource = new Dictionary<ItemType, float>();
        foreach (Vector2Int pos in Helpers.GetHexesInRange(this.Owner.GridPosition, this.CollectionRange))
        {
            var hex = Managers.Board.GetHex(pos);

            if (hex != null && BiomesCollectedFrom.ContainsKey(hex.Biome))
            {
                ItemType collectedItem = BiomesCollectedFrom[hex.Biome];

                if (!SecondsPerResourceCollection.ContainsKey(collectedItem))
                {
                    SecondsPerResourceCollection[collectedItem] = BaseSecondsPerResource[collectedItem];
                }
                else
                {
                    SecondsPerResourceCollection[collectedItem] *= .75f;
                }
            }
        }
    }

    private float lastHarvestCheckTime;
    private Dictionary<ItemType, float> lastCollectionTimes = new Dictionary<ItemType, float>();
    private void HarvestResources()
    {
        if (Time.time < lastHarvestCheckTime + .25f)
        {
            return;
        }
        lastHarvestCheckTime = Time.time;

        foreach (ItemType resource in this.BaseSecondsPerResource.Keys)
        {
            if (!lastCollectionTimes.ContainsKey(resource))
            {
                lastCollectionTimes[resource] = 0f;
            }

            if (Time.time - lastCollectionTimes[resource] > BaseSecondsPerResource[resource] &&
                this.Inventory.CanAcceptItem(resource))
            {
                Item item = ItemGenerator.Make(resource);
                this.Inventory.AddItem(item);
                lastCollectionTimes[resource] = Time.time;
            }

            int firstItemIndex = this.Inventory.FirstNonEmptyIndex();
            if (firstItemIndex != -1 &&
                this.Owner.ConveyorCell.CanAccept(
                    this.Owner.ConveyorCell.OutputBelt,
                    this.Inventory.ItemAt(firstItemIndex).Width))
            {
                Item itemToPlace = this.Inventory.ItemAt(firstItemIndex);
                SpawnItem(itemToPlace);
                this.Inventory.RemoveAt(firstItemIndex);
            }
        }
    }

    private void SpawnItem(Item item)
    {
        var resourceGO = GameObject.Instantiate(
            Prefabs.GetResource(item.Type),
            this.Owner.transform.position,
            new Quaternion());

        InstantiatedItem itemInst = resourceGO.AddComponent<InstantiatedItem>();
        itemInst.Init(item);
        this.Owner.ConveyorCell.AddItem(this.Owner.ConveyorCell.OutputBelt, itemInst);
    }
}
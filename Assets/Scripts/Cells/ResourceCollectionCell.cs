using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceCollectionCell : Cell
{
    public abstract Dictionary<ItemType, float> SecondsPerResourceCollection { get; }
    public virtual InventoryCell Inventory => outputInventory;
    private InventoryCell outputInventory;

    public override void Setup(Character character)
    {
        outputInventory = new InventoryCell(3, "Inventory");

        base.Setup(character);
    }

    public override void Update()
    {
        HarvestResources();
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

        foreach (ItemType resource in this.SecondsPerResourceCollection.Keys)
        {
            if (!lastCollectionTimes.ContainsKey(resource))
            {
                lastCollectionTimes[resource] = 0f;
            }

            if (Time.time - lastCollectionTimes[resource] > SecondsPerResourceCollection[resource] && 
                this.Inventory.CanAcceptItem(resource))
            {
                Item item = ItemGenerator.Make(resource);
                this.Inventory.AddItem(item);
                lastCollectionTimes[resource] = Time.time;
            }

            int firstItemIndex = this.Inventory.FirstNonEmptyIndex();
            if (firstItemIndex != -1 && 
                this.Owner.ConveyorCell.CanAccept(
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
        this.Owner.ConveyorCell.AddItem(itemInst);
    }
}
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceCollectionCell : Cell
{
    public abstract Dictionary<ItemType, float> SecondsPerResourceCollection { get; }

    public override void Setup(Character character)
    {
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

            if (this.Owner.ConveyorCell.CanAcceptItem() &&
                Time.time - lastCollectionTimes[resource] > SecondsPerResourceCollection[resource])
            {
                SpawnResource(resource);
                lastCollectionTimes[resource] = Time.time;
            }
        }
    }

    private void SpawnResource(ItemType type)
    {
        var resourceGO = GameObject.Instantiate(
            Prefabs.GetResource(type),
            this.Owner.transform.position,
            new Quaternion());

        Item item = ItemGenerator.GetItemScript(type);
        InstantiatedItem itemInst = resourceGO.AddComponent<InstantiatedItem>();
        itemInst.Init(item);
        this.Owner.ConveyorCell.AddItem(itemInst);
    }
}
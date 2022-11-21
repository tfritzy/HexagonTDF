using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceCollectionCell : Cell
{
    public abstract Dictionary<ResourceType, float> SecondsPerResourceCollection {get; }

    public override void Setup(Character character)
    {
        base.Setup(character);
    }

    public override void Update()
    {
        HarvestResources();
    }

    private float lastHarvestCheckTime;
    private Dictionary<ResourceType, float> lastCollectionTimes = new Dictionary<ResourceType, float>();
    private void HarvestResources()
    {
        if (Time.time < lastHarvestCheckTime + .25f)
        {
            return;
        }
        lastHarvestCheckTime = Time.time;

        foreach (ResourceType resource in this.SecondsPerResourceCollection.Keys)
        {
            if (!lastCollectionTimes.ContainsKey(resource))
            {
                lastCollectionTimes[resource] = 0f;
            }

            if (Time.time - lastCollectionTimes[resource] > SecondsPerResourceCollection[resource])
            {
                SpawnResource(resource);
                lastCollectionTimes[resource] = Time.time;
            }
        }
    }

    private void SpawnResource(ResourceType type)
    {
        var resourceGO = GameObject.Instantiate(
            Prefabs.GetResource(type),
            this.Owner.transform.position,
            new Quaternion());
        Resource resource = resourceGO.AddComponent<Resource>();
        resource.Init(type);
        this.Owner.ConveyorCell.AddItem(resource);
    }
}
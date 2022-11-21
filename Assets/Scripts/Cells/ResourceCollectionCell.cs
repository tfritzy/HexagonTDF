using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceCollectionCell : Cell
{
    public abstract Dictionary<ResourceType, float> SecondsPerResourceCollection {get; }

    private Conveyer Output;

    public override void Setup(Character character)
    {
        base.Setup(character);
    }

    public override void Update()
    {
        HarvestResources();
        LookForOutputHex();
    }

    private float lastHarvestCheckTime;
    private Dictionary<ResourceType, float> lastCollectionTimes = new Dictionary<ResourceType, float>();
    private void HarvestResources()
    {
        if (this.Output == null)
        {
            return;
        }

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
        GameObject.Instantiate(Prefabs.GetResource(type), Output.transform.position, new Quaternion());
    }

    private float lastOutputCheckTime;
    private void LookForOutputHex()
    {
        if (Time.time < lastOutputCheckTime + .5f)
        {
            return;
        }
        lastOutputCheckTime = Time.time;

        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighbor = Helpers.GetNeighborPosition(this.Owner.GridPosition, i);
            Building building = Managers.Board.GetBuilding(neighbor);
            if (building?.Type == BuildingType.Conveyor)
            {
                this.Output = (Conveyer)building;
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceCollector : Building
{
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public abstract HashSet<HexagonType> CollectionTypes { get; }
    public abstract int CollectionRatePerHex { get; }
    public abstract List<ResourceType> ResourceTypes { get; }
    public abstract int CollectionRange { get; }
    private const float BASE_TIME_BETWEEN_COLLECTIONS = 5f;
    private int numResourceHexInRange;
    private float timeBetweenResourceAdds;

    protected override void Setup()
    {
        List<Vector2Int> hexesInRange = Helpers.GetPointsInRange(this.Position, CollectionRange);
        foreach (Vector2Int pos in hexesInRange)
        {
            if (CollectionTypes.Contains(Managers.Map.Hexagons[pos.x, pos.y].Type))
            {
                numResourceHexInRange += 1;
            }
        }

        Debug.Log($"Resource collector created collecting from {numResourceHexInRange} hexes");

        timeBetweenResourceAdds = BASE_TIME_BETWEEN_COLLECTIONS / (CollectionRatePerHex * numResourceHexInRange);

        base.Setup();
    }

    private float lastCollectionTime;
    public virtual void Harvest()
    {
        if (Time.time > lastCollectionTime + timeBetweenResourceAdds)
        {
            foreach (ResourceType type in ResourceTypes)
            {
                Managers.ResourceStore.Add(type, 1);
            }

            lastCollectionTime = Time.time;
        }
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();
        Harvest();
    }
}
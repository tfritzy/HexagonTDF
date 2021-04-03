using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceCollector : Building
{
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public abstract HashSet<HexagonType> CollectionTypes { get; }
    public abstract int CollectionRatePerHex { get; }
    public abstract ResourceType CollectionType { get; }
    public abstract int CollectionRange { get; }
    private const float BASE_TIME_BETWEEN_COLLECTIONS = 1f;
    private int numResourceHexInRange;

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

        base.Setup();
    }

    private float lastCollectionTime;
    public virtual void Harvest()
    {
        if (Time.time > lastCollectionTime + BASE_TIME_BETWEEN_COLLECTIONS)
        {
            Managers.ResourceStore.Add(CollectionType, CollectionRatePerHex * numResourceHexInRange);
            lastCollectionTime = Time.time;
        }
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();
        Harvest();
    }
}
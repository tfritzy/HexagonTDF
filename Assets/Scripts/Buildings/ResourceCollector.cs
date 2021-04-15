using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceCollector : Building
{
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public abstract HashSet<HexagonType> HarvestedHexagonTypes { get; }
    public abstract int CollectionRatePerHex { get; }
    public abstract ResourceType CollectedResource { get; }
    public abstract int CollectionRange { get; }
    protected abstract int ExpectedTileCollectionCount { get; }
    private const float BASE_TIME_BETWEEN_COLLECTIONS = 5f;
    private const int EXPECTED_GAME_DURATION_SECONDS = 300;
    private const float PRODUCTION_STRUCTURE_POWER_RATIO = .2f;
    private int numResourceHexInRange;
    private float timeBetweenResourceAdds;

    protected override void Setup()
    {
        List<Vector2Int> hexesInRange = Helpers.GetAllHexInRange(this.Position, CollectionRange);
        foreach (Vector2Int pos in hexesInRange)
        {
            if (IsHarvestable(pos))
            {
                numResourceHexInRange += 1;
            }
        }

        Debug.Log($"Resource collector created collecting from {numResourceHexInRange} hexes");

        timeBetweenResourceAdds = BASE_TIME_BETWEEN_COLLECTIONS / (CollectionRatePerHex * numResourceHexInRange);

        base.Setup();
    }

    public bool IsHarvestable(Vector2Int hex)
    {
        return HarvestedHexagonTypes.Contains(Managers.Map.Hexagons[hex.x, hex.y].Type) && Managers.Map.Buildings.ContainsKey(hex) == false;
    }

    private float lastCollectionTime;
    public virtual void Harvest()
    {
        if (Time.time > lastCollectionTime + timeBetweenResourceAdds)
        {
            Managers.ResourceStore.Add(CollectedResource, 1);

            lastCollectionTime = Time.time;
        }
    }

    public override float Power
    {
        get
        {
            float power = 0f;
            int amountOfResourcesCollected =
                ExpectedTileCollectionCount *
                CollectionRatePerHex *
                ((int)(EXPECTED_GAME_DURATION_SECONDS / BASE_TIME_BETWEEN_COLLECTIONS));
            power += (amountOfResourcesCollected / Constants.ResourcePowerMap[CollectedResource]) * PRODUCTION_STRUCTURE_POWER_RATIO;
            return power;
        }
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();
        Harvest();
    }
}
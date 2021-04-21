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
    protected List<Vector2Int> HexesBeingHarvested;
    private const float BASE_TIME_BETWEEN_COLLECTIONS = 5f;
    private const float EXPECTED_GAME_DURATION_SECONDS = 600f;
    private const float PRODUCTION_STRUCTURE_POWER_RATIO = .2f;
    private int numResourceHexInRange;
    private float timeBetweenResourceAdds;

    public int CollectionRate
    {
        get { return numResourceHexInRange * CollectionRatePerHex; }
    }

    protected override void Setup()
    {
        List<Vector2Int> hexesInRange = Helpers.GetAllHexInRange(this.Position, CollectionRange);
        HexesBeingHarvested = new List<Vector2Int>();
        foreach (Vector2Int pos in hexesInRange)
        {
            if (IsHarvestable(pos))
            {
                numResourceHexInRange += 1;
                HexesBeingHarvested.Add(pos);
            }
        }

        base.Setup();
    }

    public bool IsHarvestable(Vector2Int hex)
    {
        return HarvestedHexagonTypes.Contains(Managers.Map.Hexagons[hex.x, hex.y].Type) &&
        (Managers.Map.Buildings.ContainsKey(hex) == false || Managers.Map.Buildings[hex] == this);
    }

    public override float Power
    {
        get
        {
            if (CollectedResource == ResourceType.Food)
            {
                float collectionRate = CollectionRatePerHex * ExpectedTileCollectionCount;
                float powerFoodCost = Constants.ResourcePowerMap[ResourceType.Food];
                return collectionRate / powerFoodCost;
            }

            float power = 0f;
            int amountOfResourcesCollected =
                ExpectedTileCollectionCount *
                CollectionRatePerHex *
                ((int)(EXPECTED_GAME_DURATION_SECONDS / BASE_TIME_BETWEEN_COLLECTIONS));
            power += (amountOfResourcesCollected / Constants.ResourcePowerMap[CollectedResource]) * PRODUCTION_STRUCTURE_POWER_RATIO;
            return power;
        }
    }
}
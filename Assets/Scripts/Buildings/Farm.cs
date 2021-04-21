using System.Collections.Generic;
using UnityEngine;

public class Farm : ResourceCollector
{
    public override BuildingType Type => BuildingType.Farm;
    public override HashSet<HexagonType> HarvestedHexagonTypes => hexagonTypes;
    public override int CollectionRatePerHex => 10;
    public override ResourceType CollectedResource => ResourceType.Food;
    public override int CollectionRange => 1;
    private readonly HashSet<HexagonType> hexagonTypes = new HashSet<HexagonType>() { HexagonType.Grass };
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    protected override int ExpectedTileCollectionCount => 6;

    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        { ResourceType.Wood, 1f},
    };

    protected override void Setup()
    {
        base.Setup();
        foreach (Vector2Int pos in HexesBeingHarvested)
        {
            if (pos == this.Position)
            {
                continue;
            }

            Instantiate(Prefabs.Buildings[BuildingType.GrainField], Managers.Map.Hexagons[pos.x, pos.y].transform.position, new Quaternion(), this.transform);
        }
    }
}
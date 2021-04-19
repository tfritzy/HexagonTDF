using System.Collections.Generic;

public class Lumbermill : ResourceCollector
{
    public override BuildingType Type => BuildingType.Lumbermill;
    public override HashSet<HexagonType> HarvestedHexagonTypes => hexagonTypes;
    public override int CollectionRatePerHex => 1;
    public override ResourceType CollectedResource => ResourceType.Wood;
    public override int CollectionRange => 1;
    private readonly HashSet<HexagonType> hexagonTypes = new HashSet<HexagonType>() { HexagonType.Forrest };
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    protected override int ExpectedTileCollectionCount => 2;

    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        { global::ResourceType.Wood, 1f},
    };
}
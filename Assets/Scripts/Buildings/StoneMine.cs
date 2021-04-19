using System.Collections.Generic;

public class StoneMine : ResourceCollector
{
    public override BuildingType Type => BuildingType.StoneMine;
    public override HashSet<HexagonType> HarvestedHexagonTypes => hexagonTypes;
    public override int CollectionRatePerHex => 4;
    public override ResourceType CollectedResource => ResourceType.Stone;
    public override int CollectionRange => 0;
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    protected override int ExpectedTileCollectionCount => 1;

    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        { ResourceType.Wood, 1f},
    };
    private readonly HashSet<HexagonType> hexagonTypes = new HashSet<HexagonType>() { HexagonType.Stone };
}
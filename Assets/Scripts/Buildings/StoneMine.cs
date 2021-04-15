using System.Collections.Generic;

public class StoneMine : ResourceCollector
{
    public override BuildingType Type => BuildingType.StoneMine;
    public override HashSet<HexagonType> HarvestedHexagonTypes => hexagonTypes;
    public override int CollectionRatePerHex => 6;
    public override ResourceType CollectedResource => ResourceType.Stone;
    public override int CollectionRange => 0;
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    protected override int ExpectedTileCollectionCount => 1;
    public override int PopulationCost => 2;

    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        { global::ResourceType.Wood, .9f},
        { global::ResourceType.Gold, .1f},
    };
    private readonly HashSet<HexagonType> hexagonTypes = new HashSet<HexagonType>() { HexagonType.Stone };
}
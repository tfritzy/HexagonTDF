using System.Collections.Generic;

public class Forager : ResourceCollector
{
    public override BuildingType Type => BuildingType.Forager;
    public override HashSet<HexagonType> HarvestedHexagonTypes => hexagonTypes;
    public override int CollectionRatePerHex => 1;
    public override ResourceType CollectedResource => ResourceType.Food;
    public override int CollectionRange => 2;
    private readonly HashSet<HexagonType> hexagonTypes = new HashSet<HexagonType>() { HexagonType.Grass };
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    protected override int ExpectedTileCollectionCount => 15;

    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        { global::ResourceType.Wood, 1f},
    };
}
using System.Collections.Generic;

public class Farm : ResourceCollector
{
    public override BuildingType Type => BuildingType.Farm;
    public override HashSet<HexagonType> HarvestedHexagonTypes => hexagonTypes;
    public override int CollectionRatePerHex => 10;
    public override ResourceType CollectedResource => ResourceType.Food;
    public override int CollectionRange => 1;
    public override int PopulationCost => 3;
    private readonly HashSet<HexagonType> hexagonTypes = new HashSet<HexagonType>() { HexagonType.Grass };
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    protected override int ExpectedTileCollectionCount => 6;

    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        { ResourceType.Wood, 1f},
    };
}
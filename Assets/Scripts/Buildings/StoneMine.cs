using System.Collections.Generic;

public class StoneMine : ResourceCollector
{
    public override BuildingType Type => BuildingType.StoneMine;
    public override HashSet<HexagonType> CollectionTypes => hexagonTypes;
    public override int CollectionRatePerHex => 6;
    public override List<ResourceType> ResourceTypes => resourceTypes;
    public override int CollectionRange => 0;
    public override ResourceTransaction BuildCost => new ResourceTransaction(wood: 100);
    private readonly HashSet<HexagonType> hexagonTypes = new HashSet<HexagonType>() { HexagonType.Stone };

    private List<ResourceType> resourceTypes = new List<ResourceType>() { ResourceType.Stone };
}
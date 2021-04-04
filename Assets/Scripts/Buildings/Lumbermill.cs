using System.Collections.Generic;

public class Lumbermill : ResourceCollector
{
    public override BuildingType Type => BuildingType.Lumbermill;
    public override HashSet<HexagonType> CollectionTypes => hexagonTypes;
    public override int CollectionRatePerHex => 2;
    public override List<ResourceType> ResourceTypes => resourceTypes;
    public override int CollectionRange => 1;
    private readonly HashSet<HexagonType> hexagonTypes = new HashSet<HexagonType>() { HexagonType.Forrest };
    public override ResourceTransaction BuildCost => new ResourceTransaction(wood: 50);
    private List<ResourceType> resourceTypes = new List<ResourceType>() { ResourceType.Wood };
}
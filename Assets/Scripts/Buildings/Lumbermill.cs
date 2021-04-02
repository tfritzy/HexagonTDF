using System.Collections.Generic;

public class Lumbermill : ResourceCollector
{
    public override BuildingType Type => BuildingType.Lumbermill;
    public override HashSet<HexagonType> CollectionTypes => hexagonTypes;
    public override int CollectionRatePerHex => 1;
    public override ResourceType CollectionType => ResourceType.Wood;
    public override int CollectionRange => 1;
    private readonly HashSet<HexagonType> hexagonTypes = new HashSet<HexagonType>() { HexagonType.Forrest };
}
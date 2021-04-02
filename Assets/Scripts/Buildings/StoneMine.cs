using System.Collections.Generic;

public class StoneMine : ResourceCollector
{
    public override BuildingType Type => BuildingType.StoneMine;
    public override HashSet<HexagonType> CollectionTypes => hexagonTypes;
    public override int CollectionRatePerHex => 1;
    public override ResourceType CollectionType => ResourceType.Stone;
    public override int CollectionRange => 0;
    private readonly HashSet<HexagonType> hexagonTypes = new HashSet<HexagonType>() { HexagonType.Stone };
}
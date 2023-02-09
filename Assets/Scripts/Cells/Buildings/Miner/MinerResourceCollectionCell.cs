using System.Collections.Generic;
using UnityEngine;

public class MinerResourceCollectionCell : ResourceCollectionCell
{
    public override List<Vector2Int> HexesCollectedFrom => _hexesCollectedFrom;
    private List<Vector2Int> _hexesCollectedFrom;
    public override bool CanHarvestFrom(HexagonMono hexagon)
    {
        return hexagon.Biome == Biome.Stone;
    }
    private CollectionDetails _biomeCollection = new CollectionDetails
    {
        Item = ItemType.Rock,
        TimeRequired = 2f,
    };
    public override CollectionDetails BaseCollectionDetails => _biomeCollection;

    public override void Setup(Character character)
    {
        _hexesCollectedFrom = new List<Vector2Int> { character.GridPosition };

        base.Setup(character);
    }
}
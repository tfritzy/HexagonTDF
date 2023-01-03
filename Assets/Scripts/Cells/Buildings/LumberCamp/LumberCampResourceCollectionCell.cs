using System.Collections.Generic;
using UnityEngine;

public class LumberCampResourceCollectionCell : ResourceCollectionCell
{
    public override List<Vector2Int> HexesCollectedFrom => _hexesCollectedFrom;
    private List<Vector2Int> _hexesCollectedFrom;
    public override bool CanHarvestFrom(Hexagon hexagon)
    {
        return hexagon.Biome == Biome.Forrest;
    }
    private CollectionDetails _biomeCollection = new CollectionDetails
    {
        Item = ItemType.Log,
        TimeRequired = 6f,
    };
    public override CollectionDetails BaseCollectionDetails => _biomeCollection;

    public override void Setup(Character character)
    {
        this._hexesCollectedFrom = Helpers.GetHexesInRange(character.GridPosition, 1);

        base.Setup(character);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class MinerResourceCollectionCell : ResourceCollectionCell
{
    public override List<Vector2Int> HexesCollectedFrom => _hexesCollectedFrom;
    private List<Vector2Int> _hexesCollectedFrom;

    private Dictionary<Biome, CollectionDetails> _biomeCollection = new Dictionary<Biome, CollectionDetails>
    {
        {
            Biome.Mountain,
            new CollectionDetails
            {
                Item = ItemType.Rock,
                TimeRequired = 2f,
            }
        }
    };
    public override Dictionary<Biome, CollectionDetails> BaseCollectionDetails => _biomeCollection;

    public override void Setup(Character character)
    {
        _hexesCollectedFrom = new List<Vector2Int> { character.GridPosition };

        base.Setup(character);
    }
}
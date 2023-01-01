using System.Collections.Generic;
using UnityEngine;

public class MainCharacterResourceCollectionCell : ResourceCollectionCell
{
    public override List<Vector2Int> HexesCollectedFrom => _hexesCollectedFrom;
    private List<Vector2Int> _hexesCollectedFrom = null;

    private Dictionary<Biome, CollectionDetails> _biomeCollection = new Dictionary<Biome, CollectionDetails>
    {
        {
            Biome.Forrest,
            new CollectionDetails
            {
                Item = ItemType.Log,
                TimeRequired = 8f,
            }
        },
        {
            Biome.Mountain,
            new CollectionDetails
            {
                Item = ItemType.Rock,
                TimeRequired = 4f,
            }
        }
    };
    public override Dictionary<Biome, CollectionDetails> BaseCollectionDetails => _biomeCollection;

    public void ChangeTargetHex(Vector2Int newTarget)
    {
        this._hexesCollectedFrom = new List<Vector2Int> { newTarget };
        InitCollectionRates();
    }
}
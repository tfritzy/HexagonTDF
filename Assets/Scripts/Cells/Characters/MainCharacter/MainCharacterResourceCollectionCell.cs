using System.Collections.Generic;
using UnityEngine;

public class MainCharacterResourceCollectionCell : ResourceCollectionCell
{
    public override List<Vector2Int> HexesCollectedFrom => _hexesCollectedFrom;
    private List<Vector2Int> _hexesCollectedFrom = null;
    public override bool CanHarvestFrom(Hexagon hexagon)
    {
        return
            hexagon.Biome == Biome.Forrest ||
            (hexagon.Biome == Biome.Mountain && hexagon.HasObstacle);
    }
    private Biome currentBiome;

    public Dictionary<Biome, CollectionDetails> BiomeCollections = new Dictionary<Biome, CollectionDetails>
    {
        {
            Biome.Forrest,
            new CollectionDetails
            {
                Item = ItemType.Log,
                TimeRequired = 4f,
            }
        },
        {
            Biome.Mountain,
            new CollectionDetails
            {
                Item = ItemType.Rock,
                TimeRequired = 1.5f,
            }
        }
    };
    public override CollectionDetails BaseCollectionDetails => BiomeCollections[currentBiome];

    public void ChangeTargetHex(Vector2Int newTarget, Biome biome)
    {
        currentBiome = biome;

        if (!BiomeCollections.ContainsKey(biome))
        {
            throw new System.Exception($"Biome {biome} is not harvestable by this character");
        }

        this._hexesCollectedFrom = new List<Vector2Int> { newTarget };
        InitCollectionRates();
    }
}
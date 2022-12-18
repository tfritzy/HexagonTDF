using System.Collections.Generic;

public class LumberCampResourceCollectionCell : ResourceCollectionCell
{
    private Dictionary<ItemType, float> resourceCollection = new Dictionary<ItemType, float>
    {
        {
            ItemType.Log,
            6
        }
    };
    public override Dictionary<ItemType, float> SecondsPerResourceCollection => resourceCollection;

    private Dictionary<Biome, ItemType> _biomeCollection = new Dictionary<Biome, ItemType>
    {
        {
            Biome.Forrest,
            ItemType.Log
        }
    };
    public override Dictionary<Biome, ItemType> BiomesCollectedFrom => _biomeCollection;
}
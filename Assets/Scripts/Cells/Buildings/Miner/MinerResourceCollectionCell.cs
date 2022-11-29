using System.Collections.Generic;

public class MinerResourceCollectionCell : ResourceCollectionCell
{
    private Dictionary<ItemType, float> resourceCollection = new Dictionary<ItemType, float>
    {
        {
            ItemType.Rock,
            2
        }
    };
    public override Dictionary<ItemType, float> SecondsPerResourceCollection => resourceCollection;
}
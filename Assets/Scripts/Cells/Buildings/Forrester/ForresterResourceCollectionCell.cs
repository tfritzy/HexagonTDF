using System.Collections.Generic;

public class ForresterResourceCollectionCell : ResourceCollectionCell
{
    private Dictionary<ItemType, float> resourceCollection = new Dictionary<ItemType, float>
    {
        {
            ItemType.Log,
            6
        }
    };
    public override Dictionary<ItemType, float> SecondsPerResourceCollection => resourceCollection;
}
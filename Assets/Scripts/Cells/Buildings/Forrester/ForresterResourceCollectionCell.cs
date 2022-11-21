using System.Collections.Generic;

public class ForresterResourceCollectionCell : ResourceCollectionCell
{
    private Dictionary<ResourceType, float> resourceCollection = new Dictionary<ResourceType, float>
    {
        {
            ResourceType.Log,
            6
        }
    };
    public override Dictionary<ResourceType, float> SecondsPerResourceCollection => resourceCollection;
}
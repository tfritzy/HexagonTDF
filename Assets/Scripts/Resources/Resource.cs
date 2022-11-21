using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour 
{
    public int Width => resourceWidths[this.Type];
    public ResourceType Type;

    public void Init(ResourceType type)
    {
        this.Type = type;
    }

    private static Dictionary<ResourceType, int> resourceWidths = new Dictionary<ResourceType, int>
    {
        { ResourceType.Log, 40 },
    };
}
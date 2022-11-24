using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public float WidthPercent => resourceWidths[this.Type] / 2;
    public ResourceType Type;

    public void Init(ResourceType type)
    {
        this.Type = type;
    }

    private static Dictionary<ResourceType, float> resourceWidths = new Dictionary<ResourceType, float>
    {
        { ResourceType.Log, .4f },
    };
}
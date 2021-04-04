using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceStore : MonoBehaviour
{
    private Dictionary<ResourceType, int> Resources;
    private Dictionary<ResourceType, Text> TextBoxes;

    void Start()
    {
        Resources = new Dictionary<ResourceType, int>()
        {
            { ResourceType.Gold, 0},
            { ResourceType.Wood, 0},
            { ResourceType.Stone, 0}
        };

        TextBoxes = new Dictionary<ResourceType, Text>();
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            Transform container = this.transform.Find(type.ToString());
            TextBoxes[type] = container.Find("Text").GetComponent<Text>();
        }

        Add(ResourceType.Wood, 110);
    }

    public void Add(ResourceType type, int amount)
    {
        Resources[type] += amount;
        TextBoxes[type].text = Resources[type].ToString();
    }

    public int GetAmount(ResourceType type)
    {
        return Resources[type];
    }
}
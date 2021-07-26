using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceStore : MonoBehaviour
{
    public bool InfiniteMonies;

    private Dictionary<ResourceType, int> Resources;
    private Dictionary<ResourceType, Text> TextBoxes;

    private Dictionary<ResourceType, int> resourceCollectionRates;
    private Dictionary<ResourceType, float> timeBetweenResourceAdds;
    private Dictionary<ResourceType, float> lastCollectionTimes;
    private const float BASE_TIME_BETWEEN_COLLECTIONS = 5f;
    private int TotalFoodCost;

    void Update()
    {
        if (InfiniteMonies)
        {
            Resources[ResourceType.Gold] = int.MaxValue;
        }
    }

    void Awake()
    {
        Resources = new Dictionary<ResourceType, int>()
        {
            { ResourceType.Gold, 0},
        };

        TextBoxes = new Dictionary<ResourceType, Text>();
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            Transform container = this.transform.Find(type.ToString());
            TextBoxes[type] = container.Find("Circle").Find("Count Box").Find("Text").GetComponent<Text>();
        }

        lastCollectionTimes = new Dictionary<ResourceType, float>();

        Add(ResourceType.Gold, 100);
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
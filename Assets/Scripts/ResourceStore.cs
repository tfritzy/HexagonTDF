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
        Harvest();

        if (InfiniteMonies)
        {
            Resources[ResourceType.Wood] = int.MaxValue;
            Resources[ResourceType.Gold] = int.MaxValue;
            Resources[ResourceType.Stone] = int.MaxValue;
            Resources[ResourceType.Food] = int.MaxValue;
        }
    }

    public float GetResourceCollectionRate(ResourceType resourceType)
    {
        if (resourceType == ResourceType.Food)
        {
            return resourceCollectionRates.ContainsKey(ResourceType.Food) ? resourceCollectionRates[ResourceType.Food] : 0;
        }
        else
        {
            return resourceCollectionRates.ContainsKey(resourceType) ? (float)resourceCollectionRates[resourceType] / BASE_TIME_BETWEEN_COLLECTIONS : 0;
        }
    }

    void Awake()
    {
        Resources = new Dictionary<ResourceType, int>()
        {
            { ResourceType.Gold, 0},
            { ResourceType.Wood, 0},
            { ResourceType.Stone, 0},
            { ResourceType.Food, 0}
        };

        TextBoxes = new Dictionary<ResourceType, Text>();
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            Transform container = this.transform.Find(type.ToString());
            TextBoxes[type] = container.Find("Circle").Find("Count Box").Find("Text").GetComponent<Text>();
        }

        lastCollectionTimes = new Dictionary<ResourceType, float>();

        Add(ResourceType.Wood, 110);
        Add(ResourceType.Gold, 0);
        Add(ResourceType.Stone, 0);
        Add(ResourceType.Food, 0);
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

    public void RecalculateResourceCollectionRates()
    {
        TotalFoodCost = 0;
        resourceCollectionRates = new Dictionary<ResourceType, int>();
        foreach (Building building in Managers.Map.Buildings.Values)
        {
            if (building is ResourceCollector)
            {
                ResourceCollector collector = (ResourceCollector)building;

                if (resourceCollectionRates.ContainsKey(collector.CollectedResource) == false)
                {
                    resourceCollectionRates[collector.CollectedResource] = 0;
                }

                resourceCollectionRates[collector.CollectedResource] += collector.CollectionRate;
            }

            if (building.Alliance == Alliances.Player)
            {
                TotalFoodCost += building.BuildCost.Costs.ContainsKey(ResourceType.Food) ? building.BuildCost.Costs[ResourceType.Food] : 0;
            }
        }

        timeBetweenResourceAdds = new Dictionary<ResourceType, float>();
        foreach (ResourceType type in resourceCollectionRates.Keys)
        {
            timeBetweenResourceAdds[type] = BASE_TIME_BETWEEN_COLLECTIONS / (float)resourceCollectionRates[type];
        }

        int foodCollectionRate = resourceCollectionRates.ContainsKey(ResourceType.Food) ? resourceCollectionRates[ResourceType.Food] : 0;
        Resources[ResourceType.Food] = foodCollectionRate - TotalFoodCost;
        SetFoodCounter();
    }

    public virtual void Harvest()
    {
        foreach (ResourceType resource in resourceCollectionRates.Keys)
        {
            if (resource == ResourceType.Food)
            {
                // Food works differently.
                continue;
            }

            if (lastCollectionTimes.ContainsKey(resource) == false)
            {
                lastCollectionTimes[resource] = Time.time;
            }

            if (Time.time > lastCollectionTimes[resource] + timeBetweenResourceAdds[resource])
            {
                Add(resource, 1);
                lastCollectionTimes[resource] = Time.time;
            }
        }
    }

    private void SetFoodCounter()
    {
        TextBoxes[ResourceType.Food].text = $"{TotalFoodCost} / {GetResourceCollectionRate(ResourceType.Food)}";
    }

}
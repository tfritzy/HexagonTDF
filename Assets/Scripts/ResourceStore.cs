using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceStore : MonoBehaviour
{
    public bool InfiniteMonies;

    private Dictionary<ResourceType, int> Resources;
    private Dictionary<ResourceType, Text> TextBoxes;
    public int MaxPopulation
    {
        get { return maxPopulation; }
    }
    public int CurrentPopulation
    {
        get { return currentPopulation; }
    }

    private int maxPopulation;
    private int currentPopulation;

    void Update()
    {
        if (InfiniteMonies)
        {
            Resources[ResourceType.Wood] = int.MaxValue;
            Resources[ResourceType.Gold] = int.MaxValue;
            Resources[ResourceType.Stone] = int.MaxValue;
            Resources[ResourceType.Food] = int.MaxValue;
        }
    }

    void Start()
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

    public void RecalculatePopulation()
    {
        currentPopulation = 0;
        maxPopulation = 0;

        foreach (Building building in Managers.Map.Buildings.Values)
        {
            currentPopulation += building.PopulationCost;
            maxPopulation += building.PopulationIncrease;
        }

        SetPopulationText();
    }

    private void SetPopulationText()
    {
        TextBoxes[ResourceType.Population].text = $"{CurrentPopulation} / {MaxPopulation}";
    }
}
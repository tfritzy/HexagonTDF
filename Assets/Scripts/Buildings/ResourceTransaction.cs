using System.Collections.Generic;

public class ResourceTransaction
{
    public Dictionary<ResourceType, int> Costs;

    public ResourceTransaction(float power, Dictionary<ResourceType, float> resourceRatio)
    {
        Costs = new Dictionary<ResourceType, int>();
        foreach (ResourceType type in resourceRatio.Keys)
        {
            Costs[type] = (int)(Constants.ResourcePowerMap[type] * (power * resourceRatio[type]));
        }
    }

    public ResourceTransaction(int wood = 0, int gold = 0, int stone = 0)
    {
        Costs = new Dictionary<ResourceType, int>()
        {
            {ResourceType.Wood, wood},
            {ResourceType.Gold, gold},
            {ResourceType.Stone, stone}
        };
    }

    public void Deduct()
    {
        foreach (ResourceType type in Costs.Keys)
        {
            Managers.ResourceStore.Add(type, -Costs[type]);
        }
    }

    public bool CanFulfill()
    {
        foreach (ResourceType type in Costs.Keys)
        {
            if (Managers.ResourceStore.GetAmount(type) < Costs[type])
            {
                return false;
            }
        }

        return true;
    }

    public override string ToString()
    {
        string strRep = "";
        foreach (ResourceType type in Costs.Keys)
        {
            strRep += $"{type}: {Costs[type]},";
        }

        return strRep;
    }
}
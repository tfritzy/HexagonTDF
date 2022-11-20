using System.Collections.Generic;

public class ResourceTransaction
{
    public Dictionary<ResourceType, int> Costs;

    public ResourceTransaction(float power, Dictionary<ResourceType, float> resourceRatio)
    {
        Costs = new Dictionary<ResourceType, int>();
        foreach (ResourceType type in resourceRatio.Keys)
        {
            Costs[type] = Helpers.RoundTo25((int)(Constants.ResourcePowerMap[type] * (power * resourceRatio[type])));
        }
    }

    public ResourceTransaction(int gold)
    {
        gold = Helpers.RoundTo25(gold);

        Costs = new Dictionary<ResourceType, int>()
        {
            {ResourceType.Gold, gold},
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
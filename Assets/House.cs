using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Building
{
    public override BuildingType Type => BuildingType.House;
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override float Power => throw new System.NotImplementedException();
    public override int PopulationCost => 0;
    public override int PopulationIncrease => 5;
    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        {ResourceType.Food, 1f},
    };
}

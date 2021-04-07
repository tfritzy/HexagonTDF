using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTower : AttackTower
{
    public override float Cooldown => AttackSpeed.Medium;
    public override int Damage => 20;
    public override float Range => RangeOptions.Medium;
    public override BuildingType Type => BuildingType.FireTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        {ResourceType.Stone, .6f},
        {ResourceType.Wood, .2f},
        {ResourceType.Gold, .2f},
    };
}

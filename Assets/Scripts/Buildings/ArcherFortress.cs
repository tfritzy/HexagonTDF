using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcherFortress : AttackTower
{
    public override BuildingType Type => BuildingType.ArcherFortress;
    public override float Cooldown => AttackSpeed.Fast;
    public override int Damage => 5;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override float Range => RangeOptions.Medium;
    public override int NumProjectiles => 5;
    public override float ProjectileStartPostionRandomness => .3f;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    protected override float ManualPowerAdjustment => -0.5f; // Making the arrows spread out means some can miss small targets.
    public override int PopulationCost => 3;
    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        {ResourceType.Wood, .8f},
        {ResourceType.Stone, .4f},
    };

    protected override void Attack()
    {
        base.Attack();
    }
}

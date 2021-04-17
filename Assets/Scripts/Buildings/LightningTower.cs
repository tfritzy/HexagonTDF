using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTower : AttackTower
{
    public override BuildingType Type => BuildingType.LightningTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override float Cooldown => AttackSpeed.Slow;
    public override int Damage => 30;
    public override float Range => RangeOptions.VeryLong;
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
    protected override float ManualPowerAdjustment => 1; // Double damage to air.
    public override int PopulationCost => 2;
    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        {ResourceType.Wood, .25f},
        {ResourceType.Stone, .75f},
    };

    protected override void Attack()
    {
        GameObject projectile = Instantiate(Prefabs.Projectiles[Type], Target.transform.position, Prefabs.Projectiles[Type].transform.rotation);

        if (Target.Region == VerticalRegion.Air || Target.Region == VerticalRegion.GroundAndAir)
        {
            Target.TakeDamage(this.Damage * 2);
        }
        else
        {
            Target.TakeDamage(this.Damage);
        }
    }
}

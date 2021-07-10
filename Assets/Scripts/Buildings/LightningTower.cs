using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTower : AttackTower
{
    public override BuildingType Type => BuildingType.LightningTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override float Cooldown => AttackSpeed.Slow;
    public override int BaseRange => RangeOptions.VeryLong;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
    public override int BaseDamage => 30;

    protected override float ManualPowerAdjustment => 1; // Double damage to air.
    protected override void Attack()
    {
        GameObject projectile = Instantiate(Prefabs.Projectiles[Type], Target.Position, Prefabs.Projectiles[Type].transform.rotation);

        if (Target.Region == VerticalRegion.Air || Target.Region == VerticalRegion.GroundAndAir)
        {
            Target.TakeDamage(this.Damage * 2, this);
        }
        else
        {
            Target.TakeDamage(this.Damage, this);
        }
    }
}

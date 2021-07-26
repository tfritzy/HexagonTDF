using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcherFortress : AttackTower
{
    public override BuildingType Type => BuildingType.ArcherFortress;
    public override float BaseCooldown => AttackSpeed.Fast;
    public override int BaseDamage => 5;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override float BaseRange => RangeOptions.Medium;
    public override int NumProjectiles => 5;
    public override float ProjectileStartPostionRandomness => .3f;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
    protected override float ManualPowerAdjustment => -0.5f; // Making the arrows spread out means some can miss small targets.
    protected override void Attack()
    {
        base.Attack();
    }
}

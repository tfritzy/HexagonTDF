using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallistaTower : AttackTower
{
    public override BuildingType Type => BuildingType.BallistaTower;
    public override float BaseCooldown => AttackSpeed.Slow;
    public override int BaseDamage => 30;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Maltov;
    public override float BaseRange => RangeOptions.VeryLong;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
    public override VerticalRegion Region => VerticalRegion.Ground;
    protected override float ProjectileSpeed => 20;
    protected override float RotationVelocityDegreesPerSec => 90;
    protected override int MaxPierceCount => int.MaxValue;
    protected override float ExpectedNumberOfEnemiesHitByEachProjectile => 2;
    protected override bool CanProjectilesMoveVertically => false;
}
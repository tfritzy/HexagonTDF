using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTower : AttackTower
{
    public override float BaseCooldown => AttackSpeed.Medium;
    public override int BaseDamage => 20;
    public override float BaseRange => RangeOptions.Medium;
    public override BuildingType Type => BuildingType.FireTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    protected override float ExplosionRadius => 0.5f;
}

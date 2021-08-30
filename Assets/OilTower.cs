using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilTower : AttackTower
{
    public override BuildingType Type => BuildingType.OilTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Maltov;
    public override float BaseCooldown => AttackSpeed.Slow;
    public override int BaseDamage => 30;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override float BaseRange => RangeOptions.Short;
    public override VerticalRegion Region => VerticalRegion.Ground;
    protected override float ExplosionRadius => 0.5f;
}

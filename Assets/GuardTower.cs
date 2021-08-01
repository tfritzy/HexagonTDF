using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardTower : AttackTower
{
    public override BuildingType Type => BuildingType.GuardTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Maltov;
    public override float BaseCooldown => AttackSpeed.Medium;
    public override int BaseDamage => 5;
    public override float BaseRange => RangeOptions.Medium;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
}

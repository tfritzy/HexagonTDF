using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : AttackTower
{
    public override BuildingType Type => BuildingType.ArcherTower;
    public override float BaseCooldown => AttackSpeed.Fast;
    public override int BaseDamage => 10;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Maltov;
    public override float BaseRange => RangeOptions.Medium;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
}
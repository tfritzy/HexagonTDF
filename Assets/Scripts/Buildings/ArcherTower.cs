using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : AttackTower
{
    public override BuildingType Type => BuildingType.ArcherTower;
    public override float BaseCooldown => AttackSpeed.Medium;
    public override int BaseDamage => 4;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override float BaseRange => RangeOptions.Medium;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardTower : AttackTower
{
    public override BuildingType Type => BuildingType.GuardTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override float Cooldown => AttackSpeed.Medium;
    public override int Damage => 5;
    public override float Range => RangeOptions.Medium;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
}

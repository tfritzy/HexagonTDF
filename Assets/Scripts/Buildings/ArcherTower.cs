using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : AttackTower
{
    public override BuildingType Type => BuildingType.ArcherTower;
    public override float Cooldown => AttackSpeed.Medium;
    public override int Damage => 4;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override float Range => RangeOptions.Medium;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
    public override bool IsVillageBuilding => false;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTower : AttackTower
{
    public override float Cooldown => AttackSpeed.Medium;
    public override int Damage => 20;
    public override float Range => RangeOptions.Medium;
    public override BuildingType Type => BuildingType.FireTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override bool IsVillageBuilding => false;
    protected override float ExplosionRadius => 0.5f;
}

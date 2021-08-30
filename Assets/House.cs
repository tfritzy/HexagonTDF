using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Building
{
    public override BuildingType Type => BuildingType.House;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Maltov;
    public override int StartingHealth => 100;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override int BaseDamage => 0;
    public override float Power => 1;
    public override float BaseRange => 0;
    public override float BaseCooldown => float.MaxValue / 2;
    public override bool IsMelee => true;
}

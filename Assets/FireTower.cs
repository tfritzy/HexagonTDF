using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTower : AttackTower
{
    public override float Cooldown => 2;
    public override int Damage => 3;
    public override float Range => 5;
    public override BuildingType Type => BuildingType.FireTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override ResourceTransaction BuildCost => new ResourceTransaction(wood: 25, gold: 150, stone: 200);
}

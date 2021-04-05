using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTower : AttackTower
{
    public override float Cooldown => 3;
    public override int Damage => 20;
    public override float Range => 3;
    public override BuildingType Type => BuildingType.FireTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override ResourceTransaction BuildCost => new ResourceTransaction(wood: 40, gold: 0, stone: 80);
}

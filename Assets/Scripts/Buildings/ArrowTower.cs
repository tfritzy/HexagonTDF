using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowTower : AttackTower
{
    public override BuildingType Type => BuildingType.ArrowTower;
    public override float Cooldown => 3f;
    public override float Damage => 5f;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override float Range => 5;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatHall : Building
{
    public override BuildingType Type => BuildingType.GreatHall;
    public override bool IsVillageBuilding => true;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override int StartingHealth => 100;
    public override float Power => int.MaxValue;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Building
{
    public override BuildingType Type => BuildingType.House;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Player;
    public override float Power => float.MaxValue;
    public override int StartingHealth => 10;
    public override bool IsVillageBuilding => true;

    protected override void Die()
    {
        Managers.Board.VillageBuildings.Remove(this);
        base.Die();
    }
}

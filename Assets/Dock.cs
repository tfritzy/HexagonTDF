using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : Building
{
    public override BuildingType Type => BuildingType.Dock;
    public override bool IsVillageBuilding => false;
    public override Alliances Alliance => Alliances.Neutral;
    public override Alliances Enemies => Alliances.Neutral;
    public override int StartingHealth => int.MaxValue / 2;
    public override float Power => float.MaxValue / 2;

    private HexagonMono shore;

    protected override void Setup()
    {
        base.Setup();

        for (int i = 0; i < 6; i++)
        {
            Vector2Int pos = Helpers.GetNeighborPosition(Managers.Board.Map, GridPosition, i);
            if (Managers.Board.GetHex(pos)?.Type == HexagonType.Shore)
            {
                shore = Managers.Board.GetHex(pos);
                break;
            }
        }


        this.transform.LookAt(shore.transform.position, Vector3.up);
    }
}

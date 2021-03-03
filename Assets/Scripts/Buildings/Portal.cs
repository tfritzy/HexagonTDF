using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Building
{
    private List<Vector2Int> pathToSource;
    public override BuildingType Type => BuildingType.Portal;
    public GameObject Dot;

    protected override void Setup()
    {
        pathToSource = Helpers.FindPath(Managers.BoardManager.Hexagons, Position, Managers.BoardManager.Source.Position);
        foreach (Vector2Int position in pathToSource)
        {
            Instantiate(Dot, Hexagon.ToWorldPosition(position) + Vector3.up, new Quaternion(), null);
            Debug.Log(position);
        }

        base.Setup();
    }
}

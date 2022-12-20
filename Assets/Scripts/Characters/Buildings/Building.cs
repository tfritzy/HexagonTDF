using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Character
{
    public override MovementCell MovementCell => null;
    public abstract BuildingType Type { get; }
    public virtual bool RequiresConfirmationToBuild => true;
    public virtual List<HexSide> ExtraSize => new List<HexSide>();

    public void Init(Vector2Int gridPos)
    {
        this.GridPosition = gridPos;
    }

    public Vector3 GetWorldPosition()
    {
        Vector3 center = Helpers.ToWorldPosition(this.GridPosition);
        foreach (HexSide side in ExtraSize)
        {
            center += Helpers.ToWorldPosition(Helpers.GetNeighborPosition(this.GridPosition, side));
        }

        center /= ExtraSize.Count + 1;

        return center;
    }
}
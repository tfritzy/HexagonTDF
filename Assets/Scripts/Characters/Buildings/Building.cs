using UnityEngine;

public abstract class Building : Character
{
    public override MovementCell MovementCell => null;
    public abstract BuildingType Type { get; }

    public void Init(Vector2Int gridPos)
    {
        this.GridPosition = gridPos;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class Trebuchet : Unit
{
    public override float Cooldown => AttackSpeed.VerySlow;
    public override int Damage => 10;
    public override int Range => int.MaxValue;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override int StartingHealth => 100;
    public override float Power => int.MaxValue / 2;
    public override VerticalRegion Region => VerticalRegion.Ground;
    protected override float ProjectileSpeed => 20;

    protected override void Setup()
    {
        base.Setup();
        RecalculatePath();
    }

    protected override bool IsInRangeOfTarget()
    {
        return true;
    }

    protected override Character FindTargetCharacter()
    {
        foreach (Building building in Managers.Board.Buildings.Values)
        {
            if (building.Alliance == this.Enemies)
            {
                return building;
            }
        }

        return null;
    }

    protected override void CalculateNextPathingPosition(Vector2Int currentPosition)
    {
        this.Waypoint = new Waypoint(this.GridPosition, this.GridPosition);
    }

    protected override void RecalculatePath()
    {
        CalculateNextPathingPosition(this.GridPosition);
    }

    protected override bool ShouldRecalculatePath()
    {
        return false;
    }
}
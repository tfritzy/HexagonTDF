using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Hero : Unit, Interactable
{
    protected List<Vector2Int> PathToTargetPosition;
    public bool IsListeningForTargetPosition;
    private int pathProgress;

    protected override void Setup()
    {
        base.Setup();
        PathToTargetPosition = new List<Vector2Int>();
    }

    public void Interact()
    {
        if (IsListeningForTargetPosition == false)
        {
            IsListeningForTargetPosition = true;
            // TODO: Show user this is listening for target position.
        }
        else
        {
            IsListeningForTargetPosition = false;
            // TODO: Remove elements showing it's looking for target position.
        }
    }

    public void InformHexWasClicked(HexagonMono hex)
    {
        if (IsListeningForTargetPosition)
        {
            FindPath(hex.GridPosition);
            IsListeningForTargetPosition = false;
        }
    }

    protected override void CalculateNextPathingPosition(Vector2Int currentPosition)
    {
        this.pathProgress += 1;

        if (this.pathProgress >= PathToTargetPosition.Count)
        {
            this.Waypoint = null;
            return;
        }

        this.Waypoint = new Waypoint(PathToTargetPosition[pathProgress - 1], PathToTargetPosition[pathProgress]);
    }

    protected override void FindTargetCharacter()
    {
    }

    protected override void RecalculatePath()
    {
        if (this.PathToTargetPosition?.Count == 0)
        {
            return;
        }

        FindPath(this.PathToTargetPosition.Last());
    }

    private void FindPath(Vector2Int targetPos)
    {
        pathProgress = 0;
        this.PathId = Managers.Board.PathingId;
        this.PathToTargetPosition = Helpers.FindPathByWalking(
            Managers.Board.Map,
            this.GridPosition,
            targetPos);

        if (this.PathToTargetPosition == null)
        {
            return;
        }

        this.Waypoint = new Waypoint(PathToTargetPosition[0], PathToTargetPosition[1]);
    }

    protected override bool ShouldRecalculatePath()
    {
        return this.PathId != Managers.Board.PathingId;
    }
}

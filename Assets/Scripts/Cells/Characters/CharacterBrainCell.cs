using UnityEngine;

public class CharacterBrainCell : BrainCell
{
    private const float MOVEMENT_SPEED = 1f;
    private const int MAX_EXTRA_NON_IDEAL_DIST = 10;

    public override void Update()
    {
        WalkTowardsTownHall();
    }

    private void WalkTowardsTownHall()
    {
        Vector2Int nextPos;
        int dist = Managers.Board.Navigation.GetDistanceToTownHall(this.Owner.GridPosition);
        int idealDist = Managers.Board.Navigation.GetIdealDistanceToTownHall(this.Owner.GridPosition);
        if (dist > idealDist + MAX_EXTRA_NON_IDEAL_DIST)
        {
            nextPos = Managers.Board.Navigation.GetIdealNextPos(this.Owner.GridPosition);
        }
        else
        {
            nextPos = Managers.Board.Navigation.GetNextPos(this.Owner.GridPosition);
        }

        if (!Helpers.IsInBounds(nextPos, Managers.Board.Dimensions))
        {
            return;
        }

        Vector3 diff = Helpers.ToWorldPosition(nextPos) - this.Owner.transform.position;

        if (diff.magnitude < .1f)
        {
            this.Owner.GridPosition = nextPos;
        }

        this.Owner.transform.position += diff.normalized * Time.deltaTime * MOVEMENT_SPEED;
    }
}
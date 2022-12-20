using UnityEngine;

public class CharacterBrainCell : BrainCell
{
    private const float MOVEMENT_SPEED = 1f;

    public override void Update()
    {
        WalkTowardsTownHall();
    }

    private void WalkTowardsTownHall()
    {
        Vector2Int nextPos = Managers.Board.Navigation.GetNextPos(this.Owner.GridPosition);
        Vector3 diff = Helpers.ToWorldPosition(nextPos) - this.Owner.transform.position;

        if (diff.magnitude < .01f)
        {
            this.Owner.GridPosition = nextPos;
        }

        this.Owner.transform.position += diff.normalized * Time.deltaTime * MOVEMENT_SPEED;
    }
}
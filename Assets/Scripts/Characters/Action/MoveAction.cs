using System.Collections.Generic;
using UnityEngine;

public class MoveAction : CharacterAction
{
    public LinkedList<Vector2Int> PathToFollow;
    public override MainCharacterAnimationState Animation => MainCharacterAnimationState.Running;
    private bool StopOneBefore;

    public MoveAction(Character owner, Vector2Int targetPos, bool stopOneBefore = false) : base(owner)
    {
        LinkedList<Vector2Int> path = Managers.Board.ShortestPathBetween(owner.GridPosition, targetPos);
        this.StopOneBefore = stopOneBefore;

        // if (path != null && path.Count > 0 && stopOneBefore)
        // {
        //     path.RemoveLast();
        // }

        this.PathToFollow = path;
    }

    public override void Update()
    {
        FollowPath();
    }

    private void FollowPath()
    {
        if (PathToFollow != null && PathToFollow.Count > 0)
        {
            Vector2Int nextPos = PathToFollow.First.Value;
            Vector3 delta = Helpers.ToWorldPosition(nextPos) - this.Owner.transform.position;

            if (delta.magnitude < .2f)
            {
                PathToFollow.RemoveFirst();

                if (PathToFollow.Count == 0)
                {
                    this.End();
                    return;
                }
            }

            if (this.Owner.GridPosition != Helpers.ToGridPosition(this.Owner.transform.position))
            {
                Vector2Int newPos = Helpers.ToGridPosition(this.Owner.transform.position);

                if (StopOneBefore && newPos == this.PathToFollow.Last.Value)
                {
                    // Stop on the edge of the target pos.
                    this.End();
                    return;
                }

                this.Owner.GridPosition = newPos;
            }

            Vector3 movementVector = delta.normalized * Time.deltaTime * this.Owner.MovementCell.MovementSpeed;
            this.Owner.transform.position += movementVector;
            this.Owner.transform.LookAt(this.Owner.transform.position + movementVector);
        }
        else
        {
            this.End();
        }
    }
}
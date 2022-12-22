using UnityEngine;

public class CharacterBrainCell : BrainCell
{
    private const float MOVEMENT_SPEED = 1f;
    private const int MAX_EXTRA_NON_IDEAL_DIST = 10;
    protected virtual float FindTargetRadius => 5f;

    public override void Update()
    {
        CheckForTargets();

        if (this.Target == null)
        {
            WalkTowardsTownHall();
        }
        else
        {
            AttackTarget();
        }
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

        if (diff.magnitude < .3f)
        {
            this.Owner.GridPosition = nextPos;
        }

        this.Owner.Rigidbody.MovePosition(
            this.Owner.transform.position + diff.normalized * Time.deltaTime * MOVEMENT_SPEED);
    }

    private void AttackTarget()
    {
        Vector3 diff = this.Target.transform.position - this.Owner.transform.position;
        this.Owner.Rigidbody.MovePosition(
            this.Owner.transform.position + diff.normalized * Time.deltaTime * MOVEMENT_SPEED);
    }

    private float lastTargetCheckTime;
    private void CheckForTargets()
    {
        if (Time.time < lastTargetCheckTime + .5f)
        {
            return;
        }
        lastTargetCheckTime = Time.time;

        Collider[] hits = Physics.OverlapSphere(
            this.Owner.transform.position,
            FindTargetRadius,
            Constants.Layers.Buildings);

        if (hits.Length > 0)
        {
            this.Target = hits[0].GetComponent<Character>();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class UnitBrainCell : BrainCell
{
    private const float MOVEMENT_SPEED = .5f;
    private const int MAX_EXTRA_NON_IDEAL_DIST = 10;
    protected virtual float FindTargetRadius => 5f;
    protected Unit UnitOwner => (Unit)this.Owner;
    private LinkedList<Vector2Int> pathBeingFollowed;

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
        if (Managers.Board == null)
        {
            return;
        }

        if (pathBeingFollowed == null)
        {
            pathBeingFollowed = Managers.Board.ShortestPathBetween(this.Owner.GridPosition, Managers.Board.TownHall.GridPosition);
        }

        if (pathBeingFollowed.Count <= 0)
        {
            return;
        }

        // if (!Helpers.IsInBounds(pathBeingFollowed.First.Value, Managers.Board.Dimensions))
        // {
        //     return;
        // }

        Vector3 diff = Helpers.ToWorldPosition(pathBeingFollowed.First.Value) - this.UnitOwner.transform.position;

        if (diff.magnitude < .3f)
        {
            this.pathBeingFollowed.RemoveFirst();
        }

        Vector2Int currentPos = Helpers.ToGridPosition(this.Owner.transform.position);
        this.Owner.GridPosition = currentPos;
        UpdateHeight();

        this.UnitOwner.Rigidbody.velocity = Vector3.Lerp(
            this.UnitOwner.Rigidbody.velocity,
            diff.normalized * MOVEMENT_SPEED,
            Time.deltaTime);
        this.UnitOwner.transform.LookAt(this.UnitOwner.transform.position + this.UnitOwner.Rigidbody.velocity);
    }

    private void AttackTarget()
    {
        UpdateHeight();

        Vector3? moveDir = FirstUnobstructedDirection(this.Target.transform.position - this.UnitOwner.transform.position);
        if (moveDir != null)
        {
            Vector3 newDirection = Vector3.Lerp(
                this.UnitOwner.Rigidbody.velocity,
                moveDir.Value.normalized * MOVEMENT_SPEED,
                Time.deltaTime * 3);
            this.UnitOwner.Rigidbody.velocity = newDirection;
            this.UnitOwner.transform.LookAt(this.UnitOwner.transform.position + newDirection);
            this.UnitOwner.SetAnimationState(UnitAnimationState.Walking);
        }
        else
        {
            this.UnitOwner.Rigidbody.velocity = Vector3.zero;
            this.UnitOwner.transform.LookAt(this.Target.transform);
            this.UnitOwner.SetAnimationState(UnitAnimationState.Attacking);
        }
    }

    private void UpdateHeight()
    {
        // Update grid pos and y.
        Vector2Int currentPos = Helpers.ToGridPosition(this.Owner.transform.position);
        Vector3 worldPos = this.UnitOwner.transform.position;
        worldPos.y = Managers.Board.GetHex(currentPos).transform.position.y;
        this.UnitOwner.transform.position = worldPos;
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
            this.UnitOwner.transform.position,
            FindTargetRadius,
            Constants.Layers.Buildings);

        if (hits.Length > 0)
        {
            this.Target = hits[0].GetComponent<Character>();
        }
    }


    struct DesirableDirection
    {
        public float Weight;
        public Vector3 Vector;
        public float Angle;
    };
    private const int NumSteps = 12;
    private const int AngleStep = 360 / NumSteps;
    DesirableDirection[] directions = new DesirableDirection[NumSteps];
    private const float DESIRED_DISTANCE = 0.25f;
    private const float CHECK_DISTANCE = 1f;
    private Vector3? FirstUnobstructedDirection(Vector3 desiredDirection)
    {
        desiredDirection.Normalize();
        for (int i = 0; i < NumSteps; i++)
        {
            directions[i].Angle = i * AngleStep;
            directions[i].Vector = Quaternion.AngleAxis(directions[i].Angle, Vector3.up) * desiredDirection;
            directions[i].Weight = Vector3.Dot(desiredDirection, directions[i].Vector);
        }

        Collider[] nearbyCharacters = Physics.OverlapSphere(
            this.UnitOwner.transform.position,
            CHECK_DISTANCE,
            Constants.Layers.Characters);
        foreach (Collider hit in nearbyCharacters)
        {
            if (hit.gameObject == this.UnitOwner.gameObject)
            {
                continue;
            }

            Vector3 toCharacter = hit.transform.position - this.UnitOwner.transform.position;
            float distToCharacter =
                toCharacter.magnitude -
                this.UnitOwner.Capsule.radius -
                hit.GetComponent<Character>().Capsule.radius;
            float percentDistToCharacter = Mathf.Min(distToCharacter / DESIRED_DISTANCE, 1f);

            if (hit.gameObject == this.Target.gameObject)
            {
                if (percentDistToCharacter < .5f)
                {
                    // We're happy where we are and don't need to move anywhere.
                    return null;
                }
                else
                {
                    continue;
                }
            }

            toCharacter.Normalize();
            float distScalingFactor = Mathf.Abs(percentDistToCharacter - 1f);

            // Decrease desire to walk in the direction of a nearby character.
            for (int i = 0; i < NumSteps; i++)
            {
                directions[i].Weight -= Vector3.Dot(toCharacter, directions[i].Vector) * distScalingFactor;
            }
        }

        // for (int i = 0; i < NumSteps; i++)
        // {
        //     int angle = i * AngleStep;
        //     Vector3 v = Quaternion.AngleAxis(angle, Vector3.up) * desiredDirection;
        //     float weight = (directions[i].Weight + 1) / 2;
        //     Debug.DrawLine(this.UnitOwner.transform.position, this.UnitOwner.transform.position + v * weight, Color.green, .01f);
        // }

        int mostDesired = -1;
        float mostDesiredWeight = float.MinValue;
        for (int i = 0; i < NumSteps; i++)
        {
            int angle = i * AngleStep;
            if (directions[i].Weight > mostDesiredWeight)
            {
                mostDesiredWeight = directions[i].Weight;
                mostDesired = i;
            }
        }

        return directions[mostDesired].Vector;
    }
}
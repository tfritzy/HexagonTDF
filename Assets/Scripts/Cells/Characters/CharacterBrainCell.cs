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
        if (Managers.Board == null)
        {
            return;
        }

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

        this.Owner.Rigidbody.velocity = diff.normalized * MOVEMENT_SPEED;
    }

    private void AttackTarget()
    {
        Vector3 moveDir = FirstUnobstructedDirection(this.Target.transform.position - this.Owner.transform.position);
        this.Owner.Rigidbody.velocity = moveDir.normalized * MOVEMENT_SPEED;
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

    private const int NumSteps = 12;
    private const int AngleStep = 360 / NumSteps;
    float[] weights = new float[NumSteps];
    private Vector3 FirstUnobstructedDirection(Vector3 desiredDirection)
    {
        desiredDirection.Normalize();
        for (int i = 0; i < NumSteps; i++)
        {
            int angle = i * AngleStep;
            Vector3 v = Quaternion.AngleAxis(angle, Vector3.up) * desiredDirection;
            weights[i] = Vector3.Dot(desiredDirection, v);
        }

        Collider[] nearbyCharacters = Physics.OverlapSphere(this.Owner.Position, 1f, Constants.Layers.Characters);
        foreach (Collider hit in nearbyCharacters)
        {
            if (hit.gameObject == this.Owner)
            {
                continue;
            }

            Vector3 toCharacter = hit.transform.position - this.Owner.transform.position;
            float distToCharacter = toCharacter.magnitude;
            float distScalingFactor = Mathf.Abs(distToCharacter - 1f);

            // Decrease desire to walk in the direction of a nearby character.
            for (int i = 0; i < NumSteps; i++)
            {
                int angle = i * AngleStep;
                Vector3 v = Quaternion.AngleAxis(angle, Vector3.up) * desiredDirection;
                weights[i] -= Vector3.Dot(toCharacter, v) * distScalingFactor;
            }
        }

        for (int i = 0; i < NumSteps; i++)
        {
            int angle = i * AngleStep;
            Vector3 v = Quaternion.AngleAxis(angle, Vector3.up) * desiredDirection;
            Debug.DrawLine(this.Owner.transform.position, this.Owner.transform.position + v * weights[i], Color.green, .01f);
        }

        int mostDesiredAngle = -1;
        float mostDesiredWeight = float.MinValue;
        for (int i = 0; i < NumSteps; i++)
        {
            int angle = i * AngleStep;
            if (weights[i] > mostDesiredWeight)
            {
                mostDesiredWeight = weights[i];
                mostDesiredAngle = angle;
            }
        }

        return Quaternion.AngleAxis(mostDesiredAngle * 10, Vector3.up) * desiredDirection;
    }
}
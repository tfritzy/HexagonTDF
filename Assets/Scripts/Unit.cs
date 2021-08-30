using System;
using UnityEngine;

public abstract class Unit : Character
{
    public float MovementSpeedModification;
    public float MovementSpeed;
    public override Vector3 Velocity => this.Rigidbody.velocity;

    protected Vector2Int destinationPos;
    protected virtual float BaseMovementSpeed => Constants.ENEMY_DEFAULT_MOVEMENTSPEED;
    protected virtual float DistanceFromFinalDestinationBeforeEnd => .3f;
    protected Waypoint Waypoint;
    protected Guid PathId;
    private GameObject DeathAnimation;

    protected override void Setup()
    {
        base.Setup();
        this.DeathAnimation = transform.Find("DeathAnimation")?.gameObject;
        this.MovementSpeed = BaseMovementSpeed;
        SetRagdollState(false);
    }

    protected override void UpdateLoop()
    {
        if (IsDead)
        {
            return;
        }

        base.UpdateLoop();
        if (this.AttackPhase == AttackPhase.Idle)
        {
            FollowPath();
        }
    }

    protected abstract void CalculateNextPathingPosition(Vector2Int currentPosition);
    protected abstract bool ShouldRecalculatePath();
    protected abstract void RecalculatePath();

    private void FollowPath()
    {
        if (ShouldRecalculatePath())
        {
            RecalculatePath();
        }

        if (this.Waypoint == null)
        {
            return;
        }

        Vector3 difference = (this.Waypoint.WorldspaceEndPos - this.transform.position);
        float verticalDifference = difference.y;
        difference.y = 0;

        this.Rigidbody.velocity = difference.normalized * (MovementSpeed + MovementSpeedModification);

        if (this.Rigidbody.velocity != Vector3.zero)
        {
            this.transform.rotation = Quaternion.LookRotation(this.Rigidbody.velocity, Vector3.up);
            this.CurrentAnimation = WalkAnimation;
        }

        if (difference.magnitude < Constants.HEXAGON_r)
        {
            this.GridPosition = this.Waypoint.EndPos;
        }

        if (difference.magnitude < .1f)
        {
            CalculateNextPathingPosition(this.Waypoint.EndPos);

            if (this.Waypoint == null)
            {
                this.Rigidbody.velocity = Vector3.zero;
                this.CurrentAnimation = IdleAnimation;
                return;
            }
        }
    }

    protected virtual bool IsPathBlocked()
    {
        return false;
    }

    protected override void Die()
    {
        base.Die();

        if (this.DeathAnimation != null)
        {
            this.DeathAnimation.transform.parent = null;
            Destroy(this.DeathAnimation.gameObject, 10f);

            foreach (ParticleSystem ps in this.DeathAnimation.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
        }

        SetRagdollState(true);
        DetachBody();
    }


    private void SetRagdollState(bool value)
    {
        foreach (Collider collider in this.Body.GetComponentsInChildren<Collider>())
        {
            if (1 << collider.gameObject.layer == Constants.Layers.Characters)
            {
                continue; // Need to not disable colliders on items such as shields.
            }

            collider.enabled = value;
        }

        foreach (Rigidbody rb in this.Body.GetComponentsInChildren<Rigidbody>())
        {
            if (1 << rb.gameObject.layer == Constants.Layers.Characters)
            {
                continue; // Need to not disable rigidbodies on items such as shields.
            }

            rb.isKinematic = !value;
        }
    }

    private void DetachBody()
    {
        this.Body.transform.parent = null;
        Destroy(this.Body.gameObject, 5f);
    }

    public void SetInitialPosition(Vector2Int startPos)
    {
        this.GridPosition = startPos;
    }
}
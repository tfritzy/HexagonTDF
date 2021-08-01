using System;
using UnityEngine;

public abstract class Unit : Character
{
    public float MovementSpeedModification;
    public float MovementSpeed;
    public override Vector3 Velocity => this.Rigidbody.velocity;
    public bool IsJumping;
    public AnimationState _animationState;
    public AttackPhase AttackPhase;
    public AnimationState CurrentAnimation
    {
        get
        {
            return _animationState;
        }
        set
        {
            _animationState = value;

            if (this.animator == null)
            {
                return;
            }

            this.animator.SetInteger("Animation_State", (int)_animationState);
        }
    }
    public abstract bool IsMelee { get; }

    protected Vector2Int destinationPos;
    protected virtual float BaseMovementSpeed => Constants.ENEMY_DEFAULT_MOVEMENTSPEED;
    protected virtual float DistanceFromFinalDestinationBeforeEnd => .3f;
    protected Waypoint Waypoint;
    protected Guid PathId;
    protected Character TargetCharacter;
    protected virtual AnimationState AttackAnimation => AnimationState.GeneralAttack;
    protected virtual AnimationState WalkAnimation => AnimationState.Walking;
    protected virtual AnimationState IdleAnimation => AnimationState.Idle;

    private const int MELEE_ATTACK_RANGE = 2;
    private GameObject DeathAnimation;
    private float lastAttackTime;
    private Animator animator;

    protected override void Setup()
    {
        base.Setup();
        FindTargetCharacter();
        this.animator = this.Body.GetComponent<Animator>();
        this.DeathAnimation = transform.Find("DeathAnimation")?.gameObject;
        this.MovementSpeed = BaseMovementSpeed;
        this.CurrentAnimation = IdleAnimation;
        SetRagdollState(false);
    }

    protected override void UpdateLoop()
    {
        if (IsDead)
        {
            return;
        }

        if (TargetCharacter == null)
        {
            FindTargetCharacter();
        }

        base.UpdateLoop();

        AttackTarget();
        if (this.AttackPhase == AttackPhase.Idle)
        {
            FollowPath();
        }
    }

    protected abstract void CalculateNextPathingPosition(Vector2Int currentPosition);
    protected abstract bool ShouldRecalculatePath();
    protected abstract void RecalculatePath();
    protected abstract void FindTargetCharacter();

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

    private Projectile windingUpProjectile;
    public void BeginWindup()
    {
        this.AttackPhase = AttackPhase.WindingUp;

        if (IsMelee == false && windingUpProjectile == null)
        {
            windingUpProjectile = Instantiate(
                Projectile,
                this.projectileStartPosition.position,
                new Quaternion())
                    .GetComponent<Projectile>();
            windingUpProjectile.transform.parent = this.projectileStartPosition;
        }
    }

    public void ReleaseAttack()
    {
        this.AttackPhase = AttackPhase.Recovering;

        if (!IsInRangeOfTarget())
        {
            return;
        }

        if (IsMelee)
        {
            this.TargetCharacter.TakeDamage(this.Damage, this);
        }
        else
        {
            ConfigureProjectile(windingUpProjectile);
            windingUpProjectile = null;
        }
    }

    public void FinishedRecovering()
    {
        this.AttackPhase = AttackPhase.Idle;
        this.CurrentAnimation = IdleAnimation;
    }

    protected virtual void ConfigureProjectile(Projectile projectile)
    {
        projectile.transform.parent = null;
        projectile.Initialize(DealDamageToEnemy, IsCollisionTarget, this);
        projectile.SetTracking(this.TargetCharacter.gameObject, this.ProjectileSpeed);
        projectile = null;
    }

    protected void AttackTarget()
    {
        if (Time.time > lastAttackTime + this.Cooldown && IsInRangeOfTarget() && AttackPhase == AttackPhase.Idle)
        {
            this.AttackPhase = AttackPhase.WindingUp;
            this.CurrentAnimation = this.AttackAnimation;
            this.Rigidbody.velocity = Vector3.zero;
            Vector3 diffVector = TargetCharacter.transform.position - this.transform.position;
            diffVector.y = 0;
            this.transform.rotation = Quaternion.LookRotation(diffVector, Vector3.up);
            lastAttackTime = Time.time;
        }
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

    protected virtual bool IsInRangeOfTarget()
    {
        if (TargetCharacter == null)
        {
            return false;
        }

        return (TargetCharacter.transform.position - this.transform.position).magnitude <= this.Range;
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
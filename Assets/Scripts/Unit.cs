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
    public Unit UnitBlockingPath { get; private set; }

    protected Vector2Int destinationPos;
    protected float baseMovementSpeed = Constants.ENEMY_DEFAULT_MOVEMENTSPEED;
    protected bool IsDead;
    protected virtual float DistanceFromFinalDestinationBeforeEnd => .3f;
    protected Waypoint Waypoint;
    protected Guid PathId;
    protected Character TargetCharacter;
    protected virtual AnimationState AttackAnimation => AnimationState.GeneralAttack;

    private const int MELEE_ATTACK_RANGE = 1;
    private GameObject DeathAnimation;
    private float lastAttackTime;
    private Animator animator;

    protected override void Setup()
    {
        base.Setup();
        FindTargetCharacter();
        this.animator = this.Body.GetComponent<Animator>();
        this.DeathAnimation = transform.Find("DeathAnimation")?.gameObject;
        this.MovementSpeed = baseMovementSpeed;
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

        this.UnitBlockingPath = null;
        if (TryGetCharacterBlockingPath(out Unit unit))
        {
            if (unit.UnitBlockingPath != this)
            {
                this.Rigidbody.velocity = Vector3.zero;
                this.CurrentAnimation = AnimationState.Idle;
                this.UnitBlockingPath = unit;
                return;
            }
        }

        Vector3 difference = (Managers.Board.GetHex(this.Waypoint.EndPos).transform.position - this.transform.position);
        float verticalDifference = difference.y;
        difference.y = 0;

        if (ShouldBeJumping(verticalDifference, difference))
        {
            JumpUpCliffMovement(Managers.Board.GetHex(this.Waypoint.EndPos).transform.position);
        }
        else
        {
            this.Rigidbody.velocity = difference.normalized * (MovementSpeed + MovementSpeedModification);

            if (this.Rigidbody.velocity != Vector3.zero)
            {
                this.transform.rotation = Quaternion.LookRotation(this.Rigidbody.velocity, Vector3.up);
                this.CurrentAnimation = AnimationState.Walking;
            }
        }

        if (difference.magnitude < Constants.HEXAGON_r)
        {
            this.GridPosition = this.Waypoint.EndPos;
        }

        if (difference.magnitude < .1f)
        {
            CalculateNextPathingPosition(this.Waypoint.EndPos);

            if (this.GridPosition == this.Waypoint.EndPos)
            {
                this.Rigidbody.velocity = Vector3.zero;
                this.CurrentAnimation = AnimationState.Idle;
                return;
            }
        }
    }

    private const float FORWARD_BLOCK_DISTANCE = .75f;
    private bool TryGetCharacterBlockingPath(out Unit unit)
    {
        RaycastHit[] hits = Physics.RaycastAll(
            this.transform.position,
            this.transform.forward,
            FORWARD_BLOCK_DISTANCE,
            Constants.Layers.Characters);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent<Unit>(out Unit checkUnit))
            {
                if (checkUnit == this)
                {
                    continue;
                }

                if (checkUnit.GridPosition == this.GridPosition || checkUnit.GridPosition == this.Waypoint.EndPos)
                {
                    unit = checkUnit;
                    return true;
                }
            }
        }

        unit = null;
        return false;
    }

    private Projectile windingUpProjectile;
    public void BeginWindup()
    {
        this.AttackPhase = AttackPhase.WindingUp;

        if (this.Range != MELEE_ATTACK_RANGE && windingUpProjectile == null)
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
        if (this.Range == MELEE_ATTACK_RANGE)
        {
            this.TargetCharacter.TakeDamage(this.Damage, this);
        }
        else
        {
            ConfigureProjectile(windingUpProjectile);
        }
    }

    public void FinishedRecovering()
    {
        this.AttackPhase = AttackPhase.Idle;
        this.CurrentAnimation = AnimationState.Idle;
    }

    protected virtual void ConfigureProjectile(Projectile projectile)
    {
        projectile.transform.parent = null;
        projectile.Initialize(DealDamageToEnemy, IsCollisionTarget, this);
        projectile.SetTracking(this.TargetCharacter.gameObject, this.ProjectileSpeed);
        projectile = null;
    }

    private void AttackTarget()
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
            collider.enabled = value;
        }

        foreach (Rigidbody rb in this.Body.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = !value;
        }
    }

    private bool ShouldBeJumping(float verticalDifference, Vector3 difference)
    {
        if (IsJumping)
        {
            return true;
        }

        difference.y = 0;

        return Math.Abs(verticalDifference) > .4f && difference.magnitude < Constants.HEXAGON_r * 1.6f;
    }

    private float jumpStartTime;
    private Vector3 targetJumpPosition;
    private Vector3 lastTargetJumpHexPosition;
    private float jumpUpSpeed;
    private float jumpLateralSpeed;
    private float jumpFallSpeed;
    private Vector3 jumpLateralDirection;
    private const float JUMP_MOVEMENT_DELAY_END_TIME = 0.166f;
    private const float JUMP_MOVEMENT_UP_END_TIME = 0.33f;
    private const float JUMP_PEAK_FLOAT_END_TIME = .5f;
    private const float JUMP_TOUCH_GROUND_TIME = 0.666f;
    private const float JUMP_END_TIME = 1f;
    private const float JUMP_OVERSHOOT_PERCENT = 1.2f;
    private void JumpUpCliffMovement(Vector3 targetHexPosition)
    {
        if (lastTargetJumpHexPosition != targetHexPosition)
        {
            this.CurrentAnimation = AnimationState.Jumping;
            lastTargetJumpHexPosition = targetHexPosition;
            jumpStartTime = Time.time;
            Vector3 directionVector = targetHexPosition - this.transform.position;
            directionVector.y = 0;
            targetJumpPosition = targetHexPosition + -directionVector.normalized * Constants.HEXAGON_r * .8f;
            float jumpEndHeight = targetHexPosition.y - this.transform.position.y;
            float jumpHeight = jumpEndHeight * JUMP_OVERSHOOT_PERCENT;
            jumpUpSpeed = jumpHeight / (JUMP_MOVEMENT_UP_END_TIME - JUMP_MOVEMENT_DELAY_END_TIME);
            Vector3 distToTargetPos = targetJumpPosition - this.transform.position;
            distToTargetPos.y = 0;
            jumpLateralDirection = directionVector.normalized;
            jumpLateralSpeed = distToTargetPos.magnitude / (JUMP_TOUCH_GROUND_TIME - JUMP_MOVEMENT_DELAY_END_TIME);
            jumpFallSpeed = (jumpHeight - jumpEndHeight) / (JUMP_TOUCH_GROUND_TIME - JUMP_PEAK_FLOAT_END_TIME);
            IsJumping = true;
        }

        if (Time.time < jumpStartTime + JUMP_MOVEMENT_DELAY_END_TIME)
        {
            this.Rigidbody.velocity = Vector3.zero;
        }
        else if (Time.time < jumpStartTime + JUMP_MOVEMENT_UP_END_TIME)
        {
            this.Rigidbody.velocity = Vector3.up * jumpUpSpeed + jumpLateralDirection * jumpLateralSpeed;
        }
        else if (Time.time < jumpStartTime + JUMP_PEAK_FLOAT_END_TIME)
        {
            this.Rigidbody.velocity = jumpLateralDirection * jumpLateralSpeed;
        }
        else if (Time.time < jumpStartTime + JUMP_TOUCH_GROUND_TIME)
        {
            this.Rigidbody.velocity = Vector3.down * jumpFallSpeed + jumpLateralDirection * jumpLateralSpeed;
        }
        else if (Time.time < jumpStartTime + JUMP_END_TIME)
        {
            this.Rigidbody.velocity = Vector3.zero;
        }
        else
        {
            IsJumping = false;
        }
    }

    protected virtual bool IsInRangeOfTarget()
    {
        if (TryGetCharacterBlockingPath(out Unit unit))
        {
            if (unit is Trebuchet)
            {
                return true;
            }
        }

        return false;
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
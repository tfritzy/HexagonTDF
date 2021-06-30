using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : Character
{
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public override int StartingHealth => startingHealth;
    public abstract EnemyType Type { get; }
    public abstract Dictionary<AttributeType, float> PowerToAttributeRatio { get; }
    public float MovementSpeedModification;
    public float MovementSpeed;
    public override float Power { get { return power; } }
    public bool IsOnBoat;
    public Boat Boat;
    protected bool IsDead;
    private Guid pathId;
    private GameObject DeathAnimation;
    private float baseMovementSpeed = Constants.ENEMY_DEFAULT_MOVEMENTSPEED;
    private int startingHealth;
    private float power;
    protected virtual float DistanceFromFinalDestinationBeforeEnd => .3f;
    protected Waypoint Waypoint;
    public override Vector3 Velocity => IsOnBoat ? Boat.Velocity : this.Rigidbody.velocity;
    public EnemyAnimationState _animationState;
    public EnemyAnimationState CurrentAnimation
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
    public Building TargetBuilding;
    public bool IsAttacking;
    public bool IsJumping;
    private Animator animator;
    protected Vector2Int destinationPos;
    protected abstract float Cooldown { get; }
    protected abstract int AttackDamage { get; }
    protected virtual int AttackRange => MELEE_ATTACK_RANGE;
    protected abstract EnemyAnimationState AttackAnimation { get; }
    protected Vector2Int ActualGridPosition;

    private const float VERTICAL_MOVEMENT_MODIFIER = .5f;
    private const int MELEE_ATTACK_RANGE = 1;
    private float lastAttackTime;
    private Transform projectileStartPosition;
    private LineRenderer lineRenderer;
    private Transform currentPositionOrb;

    public void SetPower(float power, float healthModifier)
    {
        this.power = power;
        this.startingHealth = (int)((this.power * PowerToAttributeRatio[AttributeType.Health]) * Constants.ENEMY_HEALTH_PER_POWER * healthModifier);
        if (startingHealth == 0)
        {
            throw new Exception("Tried to spawn an enemy with 0 health");
        }
        // this.Body.transform.localScale *= healthModifier;
        float movementSpeedPower = PowerToAttributeRatio.ContainsKey(AttributeType.MovementSpeed) ? PowerToAttributeRatio[AttributeType.MovementSpeed] : 0f;
        this.baseMovementSpeed = Constants.ENEMY_DEFAULT_MOVEMENTSPEED * (1 + movementSpeedPower);
    }

    protected override void Setup()
    {
        base.Setup();
        this.animator = this.Body.GetComponent<Animator>();
        this.DeathAnimation = transform.Find("DeathAnimation")?.gameObject;
        this.MovementSpeed = baseMovementSpeed;
        this.TargetBuilding = Managers.Board.Orbs[UnityEngine.Random.Range(0, Managers.Board.Orbs.Count)];
        this.destinationPos = TargetBuilding.GridPosition;
        this.projectileStartPosition = Helpers.RecursiveFindChild(this.transform, "ProjectileStartPosition") ?? this.transform;
        this.lineRenderer = this.GetComponent<LineRenderer>();
        this.currentPositionOrb = transform.Find("CurrentPosition");
        this.currentPositionOrb.transform.parent = null;
        SetRagdollState(false);
    }

    protected override void UpdateLoop()
    {
        if (IsDead)
        {
            return;
        }

        base.UpdateLoop();

        if (IsOnBoat)
        {
            return;
        }

        AttackTarget();
        if (IsAttacking == false)
        {
            FollowPath();
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

        int goldReward = RollGoldReward();
        Managers.ResourceStore.Add(ResourceType.Gold, goldReward);

        if (goldReward > 0)
        {
            ResourceNumber resourceNumber = Instantiate(Prefabs.ResourceNumber, Managers.Canvas).GetComponent<ResourceNumber>();
            resourceNumber.SetValue(goldReward, this.gameObject, ResourceType.Gold);
        }

        SetRagdollState(true);
        DetachBody();
    }

    public virtual void CalculatePathingPositions(Vector2Int currentPosition)
    {
        PredGridPoint nextPos = Managers.Board.GetNextStepInPathToSource(this.TargetBuilding.GridPosition, currentPosition);

        if (Managers.Board.CharacterPositions.ContainsKey(nextPos.Position))
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighborPos = Helpers.GetNeighborPosition(Managers.Board.Map, currentPosition, i);
                PredGridPoint testNextPos = Managers.Board.GetNextStepInPathToSource(this.TargetBuilding.GridPosition, neighborPos);

                if (testNextPos.Position == Constants.MaxVector2Int)
                {
                    continue;
                }

                if (testNextPos.Distance <= nextPos.Distance + 1 && Managers.Board.CharacterPositions.ContainsKey(testNextPos.Position) == false)
                {
                    nextPos = new PredGridPoint(neighborPos, testNextPos.Distance - 1);
                    break;
                }
            }
        }

        if (nextPos.Position == Constants.MaxVector2Int)
        {
            nextPos = Managers.Board.GetNextStepInPathToSource(this.TargetBuilding.GridPosition, currentPosition);
        }

        if (Managers.Board.CharacterPositions.ContainsKey(nextPos.Position) && Managers.Board.CharacterPositions[nextPos.Position] != null)
        {
            this.Rigidbody.velocity = Vector3.zero;
            this.CurrentAnimation = EnemyAnimationState.Idle;
            return;
        }

        this.Waypoint = new Waypoint();
        this.Waypoint.StartPos = currentPosition;
        this.Waypoint.EndPos = nextPos.Position;

        if (this.lineRenderer != null)
        {
            this.lineRenderer.SetPosition(0, Managers.Board.GetHex(this.Waypoint.StartPos).transform.position);
            this.lineRenderer.SetPosition(1, Managers.Board.GetHex(this.Waypoint.EndPos).transform.position);
        }
    }

    public void SetPathingPositions(Vector2Int startPos, Vector2Int endPos, bool isRecalculable)
    {
        this.Waypoint = new Waypoint(startPos, endPos, isRecalculable);
        Managers.Board.CharacterPositions[endPos] = this;
        this.GridPosition = endPos;
    }

    private void FollowPath()
    {
        if (this.TargetBuilding == null)
        {
            this.TargetBuilding = Managers.Board.Orbs[UnityEngine.Random.Range(0, Managers.Board.Orbs.Count)];
        }

        if (shouldRecalculatePath())
        {
            RecalculatePath();
        }

        // Waypoint end position is unblocked, so claim it.
        if (this.Waypoint.EndPos != this.GridPosition)
        {
            Managers.Board.CharacterPositions.Remove(this.GridPosition);
            this.GridPosition = this.Waypoint.EndPos;
            Managers.Board.CharacterPositions[this.GridPosition] = this;
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
            this.transform.rotation = Quaternion.LookRotation(this.Rigidbody.velocity, Vector3.up);
            this.CurrentAnimation = EnemyAnimationState.Walking;
        }

        if (difference.magnitude < .1f)
        {
            this.ActualGridPosition = this.Waypoint.EndPos;
            currentPositionOrb.transform.position = Managers.Board.GetHex(this.GridPosition).transform.position;
            CalculatePathingPositions(this.Waypoint.EndPos);
        }
    }

    private void AttackTarget()
    {
        if (IsInRangeOfTarget())
        {
            this.IsAttacking = true;
            this.CurrentAnimation = this.AttackAnimation;
            IsAttacking = true;
            this.Rigidbody.velocity = Vector3.zero;
            Vector3 diffVector = TargetBuilding.transform.position - this.transform.position;
            diffVector.y = 0;
            this.transform.rotation = Quaternion.LookRotation(diffVector, Vector3.up);
            lastAttackTime = Time.time;
        }
    }

    public void DealDamage()
    {
        if (this.AttackRange == MELEE_ATTACK_RANGE)
        {
            Managers.Board.Buildings[this.Waypoint.EndPos].TakeDamage(this.AttackDamage, this);
        }
        else
        {
            Projectile projectile = Instantiate(Prefabs.EnemyProjectiles[this.Type], this.projectileStartPosition.position, new Quaternion()).GetComponent<Projectile>();
            projectile.Initialize(DealDamageToEnemy, IsCollisionTarget, this);
            projectile.SetTracking(Managers.Board.Buildings[this.Waypoint.EndPos].gameObject, 5f);
        }
    }

    private bool IsCollisionTarget(Character attacker, GameObject collision)
    {
        if (collision.TryGetComponent<Building>(out Building building))
        {
            if (building == Managers.Board.Buildings[this.Waypoint.EndPos])
            {
                return true;
            }
        }

        return false;
    }

    private void DealDamageToEnemy(Character attacker, Character target, GameObject projectile)
    {
        target.TakeDamage(this.AttackDamage, this);
    }

    private bool IsInRangeOfTarget()
    {
        if (Managers.Board.Buildings.ContainsKey(this.Waypoint?.EndPos ?? Constants.MaxVector2Int) == false)
        {
            return false;
        }

        if (Helpers.IsWithinRange(this.ActualGridPosition, Managers.Board.Buildings[this.Waypoint.EndPos].GridPosition, this.AttackRange))
        {
            return true;
        }

        return false;
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
            this.CurrentAnimation = EnemyAnimationState.Jumping;
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

    private bool shouldRecalculatePath()
    {
        if (this.Waypoint.IsRecalculable)
        {
            return false;
        }

        if (Managers.Board.PathingId != this.pathId)
        {
            return true;
        }

        return false;
    }

    protected virtual void RecalculatePath()
    {
        CalculatePathingPositions(this.Waypoint.StartPos);
    }

    public int RollGoldReward()
    {
        double fullVal = ((float)Power) / (Constants.ResourcePowerMap[ResourceType.Gold] / 4); // Divide by 4 so player can build more stuff.
        double modulous = (int)fullVal > 0 ? (int)fullVal : 1;
        double randomPart = fullVal % modulous;
        return ((int)fullVal) + (UnityEngine.Random.Range(0f, 1f) <= randomPart ? 1 : 0);
    }

    public void AddRigidbody()
    {
        this.gameObject.AddComponent<Rigidbody>();
        this.Rigidbody.useGravity = false;
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

    private void DetachBody()
    {
        this.Body.transform.parent = null;
        Destroy(this.Body.gameObject, 5f);
    }
}

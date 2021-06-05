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
    private Healthbar healthbar;
    private float baseMovementSpeed = Constants.ENEMY_DEFAULT_MOVEMENTSPEED;
    private int startingHealth;
    private float power;
    protected virtual float DistanceFromFinalDestinationBeforeEnd => .3f;
    protected Vector2Int nextPathPos;
    protected Vector2Int currentPathPos;
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
    private Animator animator;
    protected Vector2Int destinationPos;

    private const float VERTICAL_MOVEMENT_MODIFIER = .5f;

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
        this.healthbar = Instantiate(Prefabs.Healthbar,
            new Vector3(10000, 10000),
            new Quaternion(),
            Managers.Canvas).GetComponent<Healthbar>();
        this.healthbar.SetOwner(this.transform);
        this.healthbar.enabled = false;
        this.TargetBuilding = Managers.Board.VillageBuildings[UnityEngine.Random.Range(0, Managers.Board.VillageBuildings.Count)];
        this.destinationPos = TargetBuilding.GridPosition;
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

        FollowPath();
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
        this.currentPathPos = currentPosition;
        this.nextPathPos = Managers.Board.GetNextStepInPathToSource(this.TargetBuilding.GridPosition, currentPathPos);
    }

    private void FollowPath()
    {
        if (this.TargetBuilding == null)
        {
            this.TargetBuilding = Managers.Board.VillageBuildings[UnityEngine.Random.Range(0, Managers.Board.VillageBuildings.Count)];
            RecalculatePath();
        }

        if (shouldRecalculatePath())
        {
            RecalculatePath();
        }

        if (nextPathPos == Constants.MaxVector2Int)
        {
            // TODO: Attack the surroundings.
            this.Rigidbody.velocity = Vector3.zero;
            this.SetMaterial(Constants.Materials.RedSeethrough);
            return;
        }

        Vector3 difference = (Managers.Board.GetHex(nextPathPos).transform.position - this.transform.position);
        float verticalDifference = difference.y;
        difference.y = 0;

        if (ShouldClimbOrDescendCliff(verticalDifference, difference))
        {
            this.Rigidbody.velocity = Math.Sign(verticalDifference) * (MovementSpeed * VERTICAL_MOVEMENT_MODIFIER) * Vector3.up;
            this.CurrentAnimation = EnemyAnimationState.ClimbingUp;
        }
        else
        {
            this.Rigidbody.velocity = difference.normalized * (MovementSpeed + MovementSpeedModification);
            this.transform.rotation = Quaternion.LookRotation(this.Rigidbody.velocity, Vector3.up);
            this.CurrentAnimation = EnemyAnimationState.Walking;
        }

        if (nextPathPos == this.destinationPos)
        {
            if (difference.magnitude < DistanceFromFinalDestinationBeforeEnd)
            {
                OnReachPathEnd();
            }
        }
        else
        {
            if (difference.magnitude < .1f)
            {
                CalculatePathingPositions(nextPathPos);
            }
        }
    }

    private bool ShouldClimbOrDescendCliff(float verticalDifference, Vector3 difference)
    {
        return Math.Abs(verticalDifference) > .01f && difference.magnitude < Constants.HEXAGON_r;
    }

    private bool shouldRecalculatePath()
    {
        return Managers.Board.PathingId != this.pathId;
    }

    protected virtual void RecalculatePath()
    {
        CalculatePathingPositions(currentPathPos);
    }

    protected virtual void OnReachPathEnd()
    {
        this.Rigidbody.velocity = Vector3.zero;
        this.TargetBuilding.TakeDamage(1);
        Destroy(this.gameObject);
        IsDead = true;
    }

    public int RollGoldReward()
    {
        double fullVal = ((float)Power) / (Constants.ResourcePowerMap[ResourceType.Gold] / 4); // Divide by 4 so player can build more stuff.
        double modulous = (int)fullVal > 0 ? (int)fullVal : 1;
        double randomPart = fullVal % modulous;
        return ((int)fullVal) + (UnityEngine.Random.Range(0f, 1f) <= randomPart ? 1 : 0);
    }

    public override void TakeDamage(int amount)
    {
        if (IsDead)
        {
            return;
        }

        DamageNumber num = Instantiate(
            Prefabs.DamageNumber,
            Vector3.zero,
            new Quaternion(),
            Managers.Canvas)
                .GetComponent<DamageNumber>();
        num.SetValue(amount, this.gameObject);

        this.healthbar.enabled = true;
        base.TakeDamage(amount);
        this.healthbar.SetFillScale((float)this.Health / (float)this.StartingHealth);
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

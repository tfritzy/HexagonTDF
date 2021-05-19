using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : Character
{
    public int PathProgress;
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
    protected List<Vector2Int> path;
    private ShoreMono shore;
    private Guid pathId;
    private GameObject DeathAnimation;
    private Healthbar healthbar;
    private float baseMovementSpeed = Constants.ENEMY_DEFAULT_MOVEMENTSPEED;
    private int startingHealth;
    private float power;
    protected virtual float DistanceFromFinalDestinationBeforeEnd => .3f;

    public void SetShore(ShoreMono shore)
    {
        this.shore = shore;
        this.pathId = shore.PathId;
        this.path = shore.PathToSource;
    }

    public override Vector3 Velocity => IsOnBoat ? Boat.Velocity : this.Rigidbody.velocity;

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
        this.PathProgress = 0;
        this.DeathAnimation = transform.Find("DeathAnimation")?.gameObject;
        this.MovementSpeed = baseMovementSpeed;
        this.healthbar = Instantiate(Prefabs.Healthbar,
            new Vector3(10000, 10000),
            new Quaternion(),
            Managers.Canvas).GetComponent<Healthbar>();
        this.healthbar.SetOwner(this.transform);
        this.healthbar.enabled = false;
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

    private void FollowPath()
    {
        RecalculatePathIfNeeded();

        if (PathProgress >= path.Count)
        {
            OnReachPathEnd();
            return;
        }

        Vector3 difference = (Map.ToWorldPosition(path[PathProgress]) - this.transform.position);
        difference.y = 0;
        this.Rigidbody.velocity = difference.normalized * (MovementSpeed + MovementSpeedModification);
        this.transform.rotation = Quaternion.LookRotation(this.Rigidbody.velocity, Vector3.up);

        if (PathProgress == path.Count - 1)
        {
            if (difference.magnitude < DistanceFromFinalDestinationBeforeEnd)
            {
                PathProgress += 1;
            }
        }
        else
        {
            if (difference.magnitude < .1f)
            {
                PathProgress += 1;
            }
        }
    }

    protected virtual void RecalculatePathIfNeeded()
    {
        if (shore.PathId != this.pathId)
        {
            RecalculatePath();
        }
    }

    protected virtual void OnReachPathEnd()
    {
        this.Rigidbody.velocity = Vector3.zero;
        Managers.Board.Source.TakeDamage(1);
        Destroy(this.gameObject);
        IsDead = true;
    }

    protected virtual void RecalculatePath()
    {
        List<Vector2Int> pathToSource = Helpers.FindPath(Managers.Board.Map, this.path[PathProgress], Managers.Board.Source.GridPosition, Helpers.IsTraversable);
        this.PathProgress = 0;
        this.pathId = shore.PathId;
        this.path = pathToSource;
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

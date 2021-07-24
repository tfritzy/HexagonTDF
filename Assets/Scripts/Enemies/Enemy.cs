using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : Unit, Interactable
{
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public abstract EnemyType Type { get; }
    public abstract Dictionary<AttributeType, float> PowerToAttributeRatio { get; }
    public override float Power { get { return power; } }
    public override int StartingHealth => startingHealth;
    public abstract float BasePower { get; }
    public bool IsEngagedInFight { get; private set; }
    public Enemy EnemyBlockingPath { get; private set; }

    private float power;
    private LineRenderer lineRenderer;
    private int startingHealth;


    public void SetPower(float power)
    {
        this.power = power;
        this.startingHealth = (int)((this.power * PowerToAttributeRatio[AttributeType.Health]) * Constants.ENEMY_HEALTH_PER_POWER);
        if (StartingHealth == 0)
        {
            throw new Exception("Tried to spawn an enemy with 0 health");
        }
        // this.Body.transform.localScale *= healthModifier;
        float movementSpeedPower = PowerToAttributeRatio.ContainsKey(AttributeType.MovementSpeed)
            ? PowerToAttributeRatio[AttributeType.MovementSpeed] : 0f;
        this.MovementSpeed = Constants.ENEMY_DEFAULT_MOVEMENTSPEED * (1 + movementSpeedPower);
    }

    protected override void UpdateLoop()
    {
        if (IsEngagedInFight)
        {
            if (this.TargetCharacter == null || this.TargetCharacter.IsDead)
            {
                this.IsEngagedInFight = false;
                this.TargetCharacter = null;
                return;
            }

            if (IsInRangeOfTarget())
            {
                AttackTarget();
            }
            else
            {
                Vector3 diffVector = (TargetCharacter.transform.position - this.transform.position).normalized;
                this.Rigidbody.velocity = diffVector * this.MovementSpeed;
            }
        }
        else
        {
            base.UpdateLoop();
        }
    }

    protected override bool IsPathBlocked()
    {
        this.EnemyBlockingPath = null;
        if (TryGetEnemyBlockingPath(out Enemy enemy))
        {
            if (enemy.EnemyBlockingPath != this && enemy.IsEngagedInFight == false)
            {
                this.Rigidbody.velocity = Vector3.zero;
                this.CurrentAnimation = IdleAnimation;
                this.EnemyBlockingPath = enemy;
                return true;
            }
        }

        return false;
    }

    private const float FORWARD_BLOCK_DISTANCE = .5f;
    private bool TryGetEnemyBlockingPath(out Enemy enemy)
    {
        RaycastHit[] hits = Physics.RaycastAll(
            this.transform.position,
            this.transform.forward,
            FORWARD_BLOCK_DISTANCE,
            Constants.Layers.Characters);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent<Enemy>(out Enemy checkEnemy))
            {
                if (checkEnemy == this)
                {
                    continue;
                }

                if (checkEnemy.GridPosition == this.GridPosition ||
                   (this.Waypoint != null && checkEnemy.GridPosition == this.Waypoint.EndPos))
                {
                    enemy = checkEnemy;
                    return true;
                }
            }
        }

        enemy = null;
        return false;
    }


    protected override void FindTargetCharacter()
    {
        this.TargetCharacter = Managers.Board.Trebuchet;
    }

    protected override void Setup()
    {
        this.SetPower(this.BasePower);
        base.Setup();
        FindTargetCharacter();
        RecalculatePath();
        this.destinationPos = this.TargetCharacter.GridPosition;
        this.lineRenderer = this.GetComponent<LineRenderer>();
    }

    protected override void Die()
    {
        int goldReward = RollGoldReward();
        Managers.ResourceStore.Add(ResourceType.Gold, goldReward);

        if (goldReward > 0)
        {
            ResourceNumber resourceNumber = Instantiate(Prefabs.ResourceNumber, Managers.Canvas).GetComponent<ResourceNumber>();
            resourceNumber.SetValue(goldReward, this.gameObject, ResourceType.Gold);
        }

        base.Die();
    }

    public void EngageInFight(Hero challenger)
    {
        IsEngagedInFight = true;
        this.TargetCharacter = challenger;
    }

    public void DisengageFromFight()
    {
        this.IsEngagedInFight = false;
        FindTargetCharacter();
    }

    protected override void CalculateNextPathingPosition(Vector2Int currentPosition)
    {
        if (currentPosition == this.TargetCharacter.GridPosition)
        {
            this.Waypoint = null;
            return;
        }

        PredGridPoint nextPos = Managers.Board.GetNextStepInPathToSource(this.TargetCharacter.GridPosition, currentPosition);

        if (nextPos.Position == Constants.MaxVector2Int)
        {
            nextPos = Managers.Board.GetNextStepInPathToSource(this.TargetCharacter.GridPosition, currentPosition);
        }

        this.Waypoint = new Waypoint();
        this.Waypoint.StartPos = currentPosition;
        this.Waypoint.EndPos = nextPos.Position;

        if (this.lineRenderer != null)
        {
            if (this.Waypoint.EndPos != Constants.MaxVector2Int && this.Waypoint.StartPos != Constants.MaxVector2Int)
            {
                this.lineRenderer.SetPosition(0, Managers.Board.GetHex(this.Waypoint.StartPos).transform.position);
                this.lineRenderer.SetPosition(1, Managers.Board.GetHex(this.Waypoint.EndPos).transform.position);
            }
        }
    }

    protected override bool ShouldRecalculatePath()
    {
        if (this.Waypoint.IsRecalculable)
        {
            return false;
        }

        if (Managers.Board.PathingId != this.PathId)
        {
            return true;
        }

        return false;
    }

    protected override void RecalculatePath()
    {
        CalculateNextPathingPosition(this.GridPosition);
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

    public bool Interact()
    {
        return Managers.Board.Hero.InformGameObjectWasClicked(this.gameObject);
    }
}

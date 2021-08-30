using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : Unit, Interactable
{
    public override Alliances Alliance => Alliances.Maltov;
    public override Alliances Enemies => Alliances.Player;
    public abstract EnemyType Type { get; }
    public abstract Dictionary<AttributeType, float> PowerToAttributeRatio { get; }
    public override float Power => BasePower * GameState.LevelPowerMultiplier;
    public override int StartingHealth => startingHealth;
    public abstract float BasePower { get; }
    public bool IsEngagedInFight { get; private set; }
    public Enemy EnemyBlockingPath { get; private set; }

    public float GoldTaperAmount;
    public Vector3 PositionOffset;

    private LineRenderer lineRenderer;
    private int startingHealth;
    private const float GOLD_PILLAGED_RATIO = .5f;
    private const float BASE_GEM_DROP_CHANCE = 3 / 829;

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
        // Testing this being disabled.
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

    private const float BLOCK_DISTANCE = .1f;
    private bool TryGetEnemyBlockingPath(out Enemy enemy)
    {
        Collider[] hits = Physics.OverlapSphere(
            this.transform.position,
            BLOCK_DISTANCE,
            Constants.Layers.Characters);

        foreach (Collider hit in hits)
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


    protected override Character FindTargetCharacter()
    {
        return Managers.Board.Trebuchet;
    }

    protected override void Setup()
    {
        this.startingHealth = (int)((this.Power * PowerToAttributeRatio[AttributeType.Health]) * Constants.ENEMY_HEALTH_PER_POWER);
        if (StartingHealth == 0)
        {
            throw new Exception("Tried to spawn an enemy with 0 health");
        }
        float movementSpeedPower = PowerToAttributeRatio.ContainsKey(AttributeType.MovementSpeed)
            ? PowerToAttributeRatio[AttributeType.MovementSpeed] : 0f;
        this.MovementSpeed = this.BaseMovementSpeed * (1 + movementSpeedPower);
        base.Setup();
        this.TargetCharacter = FindTargetCharacter();
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

        RollGemReward();

        base.Die();
    }

    private void RollGemReward()
    {
        if (UnityEngine.Random.Range(0, 1) <= BASE_GEM_DROP_CHANCE * GameState.LevelPowerMultiplier)
        {
            GameObject gem = Instantiate(
                Managers.Prefabs.Gem,
                this.transform.position + Vector3.up * .5f,
                new Quaternion(),
                null);
            gem.GetComponent<Rigidbody>().velocity = Vector3.up * 5f;
            gem.GetComponent<Rigidbody>().angularVelocity = UnityEngine.Random.insideUnitSphere * 360;
        }
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
        if (currentPosition == this.TargetCharacter?.GridPosition)
        {
            this.Waypoint = null;
            return;
        }

        PredGridPoint nextPos = Managers.Board.GetNextStepInPathToSource(this.TargetCharacter.GridPosition, currentPosition);

        if (nextPos.Position == Constants.MaxVector2Int)
        {
            nextPos = Managers.Board.GetNextStepInPathToSource(this.TargetCharacter.GridPosition, currentPosition);
        }

        this.Waypoint = new Waypoint(currentPosition, nextPos.Position, this.PositionOffset);
    }

    protected override bool ShouldRecalculatePath()
    {
        if (this.Waypoint == null)
        {
            return false;
        }

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
        return (int)(Power * Constants.ResourcePowerMap[ResourceType.Gold] * GOLD_PILLAGED_RATIO * UnityEngine.Random.Range(.5f, 1.5f) * this.GoldTaperAmount);
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

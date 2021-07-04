using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : Unit
{
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public abstract EnemyType Type { get; }
    public abstract Dictionary<AttributeType, float> PowerToAttributeRatio { get; }
    public override float Power { get { return power; } }
    public override int StartingHealth => startingHealth;

    private float power;
    private LineRenderer lineRenderer;
    private Transform currentPositionOrb;
    private int startingHealth;

    public void SetPower(float power, float healthModifier)
    {
        this.power = power;
        this.startingHealth = (int)((this.power * PowerToAttributeRatio[AttributeType.Health]) * Constants.ENEMY_HEALTH_PER_POWER * healthModifier);
        if (StartingHealth == 0)
        {
            throw new Exception("Tried to spawn an enemy with 0 health");
        }
        // this.Body.transform.localScale *= healthModifier;
        float movementSpeedPower = PowerToAttributeRatio.ContainsKey(AttributeType.MovementSpeed) ? PowerToAttributeRatio[AttributeType.MovementSpeed] : 0f;
        this.baseMovementSpeed = Constants.ENEMY_DEFAULT_MOVEMENTSPEED * (1 + movementSpeedPower);
    }

    protected override Character FindTargetCharacter()
    {
        return Managers.Board.Trebuchet;
    }

    protected override void Setup()
    {
        base.Setup();
        this.destinationPos = this.TargetCharacter.GridPosition;
        this.lineRenderer = this.GetComponent<LineRenderer>();
        this.currentPositionOrb = transform.Find("CurrentPosition");
        this.currentPositionOrb.transform.parent = null;
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

    protected override void CalculateNextPathingPosition(Vector2Int currentPosition)
    {
        PredGridPoint nextPos = Managers.Board.GetNextStepInPathToSource(this.TargetCharacter.GridPosition, currentPosition);

        if (Managers.Board.CharacterPositions.ContainsKey(nextPos.Position))
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighborPos = Helpers.GetNeighborPosition(Managers.Board.Map, currentPosition, i);
                PredGridPoint testNextPos = Managers.Board.GetNextStepInPathToSource(this.TargetCharacter.GridPosition, neighborPos);

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
            nextPos = Managers.Board.GetNextStepInPathToSource(this.TargetCharacter.GridPosition, currentPosition);
        }

        if (Managers.Board.CharacterPositions.ContainsKey(nextPos.Position) && Managers.Board.CharacterPositions[nextPos.Position] != null)
        {
            this.Rigidbody.velocity = Vector3.zero;
            this.CurrentAnimation = AnimationState.Idle;
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
        CalculateNextPathingPosition(this.Waypoint.StartPos);
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
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boat : Enemy
{
    public override EnemyType Type => EnemyType.Boat;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public int Capacity = 3;
    public List<Enemy> Passangers = new List<Enemy>();
    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, 1f},
    };
    private const string SEAT_NAME = "Seat";
    public override int StartingHealth => int.MaxValue;
    protected override float DistanceFromFinalDestinationBeforeEnd => 1.8f;
    protected override float Cooldown => float.MaxValue / 2; // Divide by 2 because cooldown gets added to time, which would overflow.
    protected override int AttackDamage => 0;
    protected override float AttackRange => 0;
    protected List<Vector2Int> pathToShore;
    protected int pathProgress;

    public void SetInitialPos(Vector2Int startGridPos)
    {
        FindNewPath(startGridPos);
        CalculatePathingPositions(startGridPos);
    }

    protected override void Setup()
    {
        base.Setup();
        this.destinationPos = pathToShore.Last();
    }

    protected override void OnReachPathEnd()
    {
        this.Rigidbody.velocity = Vector3.zero;
        KickPassangersOffBoat();
        Die();
    }

    public override void CalculatePathingPositions(Vector2Int currentPosition)
    {
        if (this.pathProgress + 1 < this.pathToShore.Count)
        {
            this.currentPathPos = this.nextPathPos;
            this.nextPathPos = this.pathToShore[this.pathProgress + 1];
            this.pathProgress += 1;
        }
    }

    protected override void RecalculatePath()
    {
        FindNewPath(this.pathToShore[this.pathProgress]);
        CalculatePathingPositions(currentPathPos);
    }

    private void FindNewPath(Vector2Int startPos)
    {
        this.pathToShore = Helpers.FindPath(
            Managers.Board.Map,
            startPos,
            new HashSet<Vector2Int>(Managers.Board.Map.Docks),
            (Vector2Int pos) => { return Managers.Board.Map.GetHex(pos).Value == HexagonType.Water; },
            isValidDock);
        this.pathProgress = 0;
        this.destinationPos = this.pathToShore.Last();
    }

    private bool isValidDock(Vector2Int pos)
    {
        return Managers.Board.Buildings.ContainsKey(pos) && Managers.Board.Buildings[pos].Type == BuildingType.Dock;
    }

    protected override void Die()
    {
        IsDead = true;
        Destroy(this.gameObject, 1f);
    }

    public void AddPassanger(Enemy passanger)
    {
        passanger.transform.position = GetSeatPos(Passangers.Count);
        passanger.Boat = this;
        Passangers.Add(passanger);
        passanger.IsOnBoat = true;
        GameObject.Destroy(passanger.GetComponent<Rigidbody>()); // Passangers cannot have rigidbodies because they won't move.
    }

    public Vector3 GetSeatPos(int index)
    {
        if (index >= Capacity)
        {
            throw new System.ArgumentException("This boat doesn't have that many seats!");
        }

        if (index < 0)
        {
            throw new System.ArgumentException("Cannot get negative seat.");
        }

        return this.transform.Find("Body").Find(SEAT_NAME + index).position;
    }

    private void KickPassangersOffBoat()
    {
        foreach (Enemy passanger in Passangers)
        {
            if (passanger == null)
            {
                // passanger could have been killed.
                continue;
            }

            passanger.transform.parent = null;
            passanger.IsOnBoat = false;
            passanger.AddRigidbody();
            passanger.CalculatePathingPositions(this.pathToShore.Last());
        }
    }
}

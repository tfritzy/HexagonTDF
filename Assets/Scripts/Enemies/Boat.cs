using System.Collections;
using System.Collections.Generic;
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
    private ShoreMono targetShore;
    private const string SEAT_NAME = "Seat";
    public override int StartingHealth => int.MaxValue;
    protected override float DistanceFromFinalDestinationBeforeEnd => 1.8f;

    public void SetInitialPos(Vector2Int startGridPos)
    {
        FindNewPath(startGridPos);
    }

    protected override void OnReachPathEnd()
    {
        this.Rigidbody.velocity = Vector3.zero;
        KickPassangersOffBoat();
        Die();
    }

    protected override void RecalculatePathIfNeeded()
    {
        if (Helpers.IsTraversable(targetShore.GridPosition) == false)
        {
            RecalculatePath();
        }
    }

    protected override void RecalculatePath()
    {
        FindNewPath(this.path[this.PathProgress]);
    }

    private void FindNewPath(Vector2Int startPos)
    {
        this.path = Helpers.FindPath(
            Managers.Board.Map,
            startPos,
            new HashSet<Vector2Int>(Managers.Board.Map.LandableShores),
            (Vector2Int pos) => { return Managers.Board.Map.GetHex(pos).Value == HexagonType.Water; },
            Helpers.IsTraversable);
        this.PathProgress = 0;
        Vector2Int targetShorePos = this.path[this.path.Count - 1];
        this.targetShore = Managers.Board.Hexagons[targetShorePos.x, targetShorePos.y].GetComponent<ShoreMono>();
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
            passanger.SetShore(this.targetShore);
        }
    }
}

using UnityEngine;

public abstract class ObstacleHexagon : Hexagon
{
    public abstract float ObstacleChance { get; }
    public abstract GameObject GetObstacle(System.Random random);
    public abstract float SizeVarience { get; }
    protected bool HasObstacle;
    private GameObject Obstacle;
    public override bool IsBuildable => !HasObstacle;
    public override bool IsWalkable => !HasObstacle;

    public void GenerateObstacle(Transform hex, Vector2Int hexGridPos)
    {
        System.Random random = new System.Random(GameState.SelectedSegment.Coordinates.GetHashCode() * 786433 + hexGridPos.GetHashCode() * 3145739);
        if (random.NextDouble() <= ObstacleChance)
        {
            HasObstacle = true;
            Obstacle = GameObject.Instantiate(GetObstacle(random), hex.transform.position, new Quaternion(), hex);
            float size = Random.Range(1 - SizeVarience, 1 + SizeVarience);
            Obstacle.transform.localScale = Obstacle.transform.localScale * size;
            Obstacle.transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
        }
    }

    public void RemoveObstacle()
    {
        HasObstacle = false;
        GameObject.Destroy(Obstacle);
    }
}
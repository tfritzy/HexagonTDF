using UnityEngine;

public abstract class ObstacleHexagon : Hexagon
{
    public abstract float ObstacleChance { get; }
    public abstract GameObject GetObstacle(System.Random random);
    public abstract float SizeVarience { get; }
    public virtual float NumObstacles => 1;
    protected bool HasObstacle;
    private GameObject Obstacle;

    protected ObstacleHexagon(int height) : base(height)
    {
    }

    public override bool IsBuildable => !HasObstacle;
    public override bool IsWalkable => !HasObstacle;

    public void GenerateObstacle(Transform hex, Vector2Int hexGridPos)
    {
        System.Random random = new System.Random(hexGridPos.GetHashCode());
        if (random.NextDouble() <= ObstacleChance)
        {
            HasObstacle = true;
            for (int i = 0; i < NumObstacles; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-Constants.HEXAGON_r * .9f, Constants.HEXAGON_r * .9f), 0, Random.Range(-Constants.HEXAGON_r * .9f, Constants.HEXAGON_r * .9f));
                Obstacle = GameObject.Instantiate(GetObstacle(random), hex.transform.position + offset, new Quaternion(), hex);
                float size = Random.Range(1 - SizeVarience, 1 + SizeVarience);
                Obstacle.transform.localScale = Obstacle.transform.localScale * size;
                Obstacle.transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
            }
        }
    }

    public void RemoveObstacle()
    {
        HasObstacle = false;
        GameObject.Destroy(Obstacle);
    }
}
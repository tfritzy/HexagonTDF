using UnityEngine;

public abstract class Hexagon
{
    public abstract Biome Biome { get; }
    public virtual bool IsBuildable => !HasObstacle;
    public virtual bool IsWalkable => !HasObstacle;
    public abstract Color BaseColor { get; }
    public float MaxColorVariance => .015f;
    public int Height;
    public virtual float ObstacleChance => 0f;
    public virtual GameObject GetObstacleBody() { return null; }
    public bool HasObstacle { get; private set; }

    public Hexagon(int height)
    {
        this.Height = height;
    }

    public void RollObstacle()
    {
        if (ObstacleChance > 0 && Random.Range(0f, 1f) < ObstacleChance)
        {
            HasObstacle = true;
        }
    }
}
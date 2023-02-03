using UnityEngine;

public abstract class Hexagon
{
    public abstract Biome Biome { get; }
    public virtual bool IsBuildable => true;
    public virtual bool IsWalkable => true;
    public abstract Color BaseColor { get; }
    public float MaxColorVariance => .015f;
    public virtual float ObstacleChance => 0f;
    public virtual GameObject GetObstacleBody() { return null; }
    public virtual int NumDecorations => 0;
    public virtual GameObject GetDecorationBody() { return null; }
    public virtual bool IsTransparent => false;

    public Hexagon()
    {
    }
}
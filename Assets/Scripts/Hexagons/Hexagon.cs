using UnityEngine;

public abstract class Hexagon
{
    public abstract Biome Biome { get; }
    public abstract bool IsBuildable { get; }
    public abstract bool IsWalkable { get; }
    public abstract Color BaseColor { get; }
    public float MaxColorVariance => .015f;
    public int Height;

    public Hexagon(int height)
    {
        this.Height = height;
    }
}
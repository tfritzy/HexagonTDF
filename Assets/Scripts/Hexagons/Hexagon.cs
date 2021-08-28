using UnityEngine;

public abstract class Hexagon
{
    public abstract Biome Biome { get; }
    public abstract bool IsBuildable { get; }
    public virtual bool IsWalkable => IsBuildable;
    public abstract Color BaseColor { get; }
    public virtual Material Material => Constants.Materials.TintableHex;
    public virtual float MaxColorVariance => .01f;
}
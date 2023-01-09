using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forrest : Hexagon
{
    public override Biome Biome => Biome.Forrest;
    public override Color BaseColor => ColorExtensions.Create("#477664");
    public override bool IsBuildable => false;
    public override bool IsWalkable => false;

    public Forrest(int height) : base(height)
    {
    }

    public override float ObstacleChance => .1f;
    public override GameObject GetObstacleBody()
    {
        return Managers.Prefabs.Trees[
            Random.Range(0, Managers.Prefabs.Trees.Length)
        ];
    }
}

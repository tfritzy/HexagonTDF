using System;
using UnityEngine;

public class Grassland : ObstacleHexagon
{
    public override Biome Biome => Biome.Grassland;
    public override bool IsBuildable => true;
    public override Color BaseColor => ColorExtensions.Create("#3E7854");
    public override float ObstacleChance => .03f;
    public override float SizeVarience => .4f;

    public override GameObject GetObstacle(System.Random random)
    {
        return Managers.Prefabs.DeadTrees[random.Next(0, Managers.Prefabs.DeadTrees.Length)];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forrest : ObstacleHexagon
{
    public override Biome Biome => Biome.Forrest;
    public override Color BaseColor => ColorExtensions.Create("#477664");
    public override float ObstacleChance => 1f;
    public override float SizeVarience => .4f;
    public override float NumObstacles => 8;
    private const float liveTreeChance = .8f;

    public Forrest(int height) : base(height)
    {
    }

    public override GameObject GetObstacle(System.Random random)
    {
        return Managers.Prefabs.Trees[random.Next(0, Managers.Prefabs.Trees.Length)];

        // if (random.NextDouble() <= liveTreeChance)
        // {
        //     return Managers.Prefabs.Trees[random.Next(0, Managers.Prefabs.Trees.Length)];
        // }
        // else
        // {
        //     return Managers.Prefabs.DeadTrees[random.Next(0, Managers.Prefabs.DeadTrees.Length)];
        // }
    }
}

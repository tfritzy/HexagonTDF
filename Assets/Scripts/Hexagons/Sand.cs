using System;
using UnityEngine;

public class Sand : ObstacleHexagon
{
    public override Biome Biome => Biome.Sand;
    public override bool IsBuildable => true;
    public override Color BaseColor => ColorExtensions.Create("#e6d18a");
    public override float ObstacleChance => .1f;
    public override float SizeVarience => .15f;

    public override GameObject GetObstacle(System.Random random)
    {
        return Managers.Prefabs.Cacti[random.Next(0, Managers.Prefabs.Cacti.Length)];
    }
}

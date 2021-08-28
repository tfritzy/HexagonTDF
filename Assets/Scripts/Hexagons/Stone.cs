using UnityEngine;

public class Mountain : ObstacleHexagon
{
    public override Biome Biome => Biome.Mountain;
    public override Color BaseColor => ColorExtensions.Create("#969284");
    public override float MaxColorVariance => .02f;
    public override float ObstacleChance => .04f;
    public override float SizeVarience => .25f;

    public override GameObject GetObstacle()
    {
        return Managers.Prefabs.StoneColumn;
    }
}

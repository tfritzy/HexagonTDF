using UnityEngine;

public class Mountain : ObstacleHexagon
{
    public override Biome Biome => Biome.Mountain;
    public override Color BaseColor => ColorExtensions.Create("#596A78");
    public override float ObstacleChance => .04f;
    public override float SizeVarience => .25f;

    public override GameObject GetObstacle(System.Random random)
    {
        return Managers.Prefabs.StoneColumns[Random.Range(0, Managers.Prefabs.StoneColumns.Length)];
    }
}

using UnityEngine;

public class Mountain : Hexagon
{
    public Mountain()
    {
    }

    public override Biome Biome => Biome.Mountain;
    public override Color BaseColor => ColorExtensions.Create("#44627B");
    public override float ObstacleChance => .1f;
    public override GameObject GetObstacleBody()
    {
        return Managers.Prefabs.StoneColumns[
            Random.Range(0, Managers.Prefabs.StoneColumns.Length)
        ];
    }
}
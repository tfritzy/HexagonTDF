using UnityEngine;

public class Grass : Hexagon
{
    public override Biome Biome => Biome.Grassland;
    public override bool IsBuildable => true;
    public override Color BaseColor => ColorExtensions.Create("#729372");
}

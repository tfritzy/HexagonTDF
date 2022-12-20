using UnityEngine;

public class Water : Hexagon
{
    public override Biome Biome => Biome.Water;
    public override bool IsBuildable => false;
    public override Color BaseColor => ColorExtensions.Create("#3d71c4");
    public override Material Material => Constants.Materials.TintableUnlit;

    public Water() : base(0) { }
}

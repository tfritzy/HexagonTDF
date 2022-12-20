using UnityEngine;

public class Snow : Hexagon
{
    public Snow(int height) : base(height)
    {
    }

    public override Biome Biome => Biome.Snow;
    public override bool IsBuildable => true;
    public override Color BaseColor => ColorExtensions.Create("#000000");
}

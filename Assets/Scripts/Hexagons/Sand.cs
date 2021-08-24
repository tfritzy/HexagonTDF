using UnityEngine;

public class Sand : Hexagon
{
    public override Biome Biome => Biome.Sand;
    public override bool IsBuildable => true;
    public override Color BaseColor => ColorExtensions.Create("#C2B280");
}

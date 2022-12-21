using UnityEngine;

public class Mountain : Hexagon
{
    public Mountain(int height) : base(height)
    {
    }

    public override Biome Biome => Biome.Mountain;
    public override Color BaseColor => ColorExtensions.Create("#44627B");
    public override bool IsBuildable => true;
    public override bool IsWalkable => true;
}

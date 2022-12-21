using System;
using UnityEngine;

public class Sand : Hexagon
{
    public Sand(int height) : base(height)
    {
    }

    public override Biome Biome => Biome.Sand;
    public override bool IsBuildable => true;
    public override bool IsWalkable => true;
    public override Color BaseColor => ColorExtensions.Create("#a2956a");
}

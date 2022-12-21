using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forrest : Hexagon
{
    public override Biome Biome => Biome.Forrest;
    public override Color BaseColor => ColorExtensions.Create("#477664");
    public override bool IsBuildable => false;
    public override bool IsWalkable => false;

    public Forrest(int height) : base(height)
    {
    }
}

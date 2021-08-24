using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forrest : Hexagon
{
    public override Biome Biome => Biome.Forrest;
    public override bool IsBuildable => false;
    public override Color BaseColor => ColorExtensions.Create("#306847");
}

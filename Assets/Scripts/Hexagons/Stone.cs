using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mountain : Hexagon
{
    public override Biome Biome => Biome.Mountain;
    public override bool IsBuildable => true;
    public override Color BaseColor => ColorExtensions.Create("#6e7a9e");
}

using System;
using UnityEngine;

public class Grassland : Hexagon
{
    public Grassland()
    {
    }

    public override Biome Biome => Biome.Grassland;
    public override Color BaseColor => ColorExtensions.Create("#477664");
}

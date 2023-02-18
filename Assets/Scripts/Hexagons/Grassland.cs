using System;
using UnityEngine;

public class Grassland : Hexagon
{
    public Grassland()
    {
    }

    public override Biome Biome => Biome.Grassland;
    public override Color BaseColor => ColorExtensions.Create("#477664");

    // public override int NumDecorations => 2;
    public override GameObject GetDecorationBody()
    {
        return Managers.Prefabs.Grass;
    }
}

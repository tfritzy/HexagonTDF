using System;
using UnityEngine;

public class Sand : Hexagon
{
    public Sand()
    {
    }

    public override Biome Biome => Biome.Sand;
    public override Color BaseColor => ColorExtensions.Create("#a2956a");
}

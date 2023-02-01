using UnityEngine;

public class Stone : Hexagon
{
    public Stone()
    {
    }

    public override Biome Biome => Biome.Stone;
    public override Color BaseColor => ColorExtensions.Create("#000000");
}

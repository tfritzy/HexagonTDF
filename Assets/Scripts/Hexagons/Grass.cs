using UnityEngine;

public class Grass : Hexagon
{
    public override HexagonType Type => HexagonType.Grass;
    public override bool IsBuildable => true;
    public override Color BaseColor => ColorExtensions.Create("#3f7f59");
}

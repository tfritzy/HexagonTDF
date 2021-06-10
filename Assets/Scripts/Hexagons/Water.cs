using UnityEngine;

public class Water : Hexagon
{
    public override HexagonType Type => HexagonType.Water;
    public override bool IsBuildable => false;
    public override Color BaseColor => ColorExtensions.Create("6798c7");
    public override Material Material => Constants.Materials.TintableUnlit;
}

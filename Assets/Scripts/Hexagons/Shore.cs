using UnityEngine;

public class Shore : Hexagon
{
    public override HexagonType Type => HexagonType.Shore;
    public override bool IsBuildable => true;
    public override bool IsWalkable => true;
    public override Color BaseColor => ColorExtensions.Create("c2b280");
}
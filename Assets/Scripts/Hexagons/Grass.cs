public class Grass : Hexagon
{
    public override HexagonType Type => HexagonType.Grass;

    public override bool IsBuildable => true;
}

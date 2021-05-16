public abstract class Hexagon
{
    public abstract HexagonType Type { get; }
    public abstract bool IsBuildable { get; }
    public virtual bool IsWalkable => IsBuildable;
}
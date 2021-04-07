public class Icid : Enemy
{
    public override int StartingHealth => 40;
    public override float MovementSpeed => 1;
    public override EnemyType Type => EnemyType.Icid;
    public override VerticalRegion Region => VerticalRegion.Ground;
}
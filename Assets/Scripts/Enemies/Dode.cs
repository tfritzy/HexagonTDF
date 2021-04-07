public class Dode : Enemy
{
    public override int StartingHealth => 20;
    public override float MovementSpeed => 1;
    public override EnemyType Type => EnemyType.Dode;
    public override VerticalRegion Region => VerticalRegion.Ground;
}
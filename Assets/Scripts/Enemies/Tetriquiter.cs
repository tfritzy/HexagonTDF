public class Tetriquiter : Enemy
{
    public override int StartingHealth => 5;
    public override float BaseMovementSpeed => 1;
    public override EnemyType Type => EnemyType.Tetriquiter;
    public override VerticalRegion Region => VerticalRegion.Ground;
}
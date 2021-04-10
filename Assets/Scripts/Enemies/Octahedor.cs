public class Octahedor : Enemy
{
    public override int StartingHealth => 80;
    public override float BaseMovementSpeed => 1;
    public override EnemyType Type => EnemyType.Octahedor;
    public override VerticalRegion Region => VerticalRegion.Ground;
}
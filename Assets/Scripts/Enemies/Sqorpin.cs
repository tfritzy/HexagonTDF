public class Sqorpin : Enemy
{
    public override int StartingHealth => 10;
    public override float BaseMovementSpeed => 1;
    public override EnemyType Type => EnemyType.Sqorpin;
    public override VerticalRegion Region => VerticalRegion.Ground;
}
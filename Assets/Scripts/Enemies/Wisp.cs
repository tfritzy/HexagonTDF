public class Wisp : Enemy
{
    public override int StartingHealth => 5;
    public override int GoldReward => 1;
    public override float MovementSpeed => 1;
    public override EnemyType Type => EnemyType.Wisp;
    public override VerticalRegion Region => VerticalRegion.Ground;
}
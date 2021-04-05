public class Tetriquiter : Enemy
{
    public override int StartingHealth => 10;
    public override int GoldReward => 2;
    public override float MovementSpeed => 1;
    public override EnemyType Type => EnemyType.Tetriquiter;
}
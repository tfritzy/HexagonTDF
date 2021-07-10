using System.Collections.Generic;

public class ShieldKnight : Enemy
{
    public override EnemyType Type => EnemyType.ShieldKnight;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override float Cooldown => AttackSpeed.Slow;
    public override int BaseDamage => 1;
    protected override AnimationState AttackAnimation => AnimationState.ShieldAttack;
    public override int BaseRange => 1;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override float BasePower => 6;
    protected override AnimationState WalkAnimation => AnimationState.ShieldWalk;
    protected override AnimationState IdleAnimation => AnimationState.ShieldIdle;

    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, .75f}, // Shield costs 25%
    };
}
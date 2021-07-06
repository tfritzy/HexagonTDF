using System.Collections.Generic;

public class ShieldKnight : Enemy
{
    public override EnemyType Type => EnemyType.ShieldKnight;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override float Cooldown => AttackSpeed.Medium;
    public override int Damage => 1;
    protected override AnimationState AttackAnimation => AnimationState.SlashingSword;
    public override int Range => 1;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override float BasePower => 6;
    protected override AnimationState WalkAnimation => AnimationState.ShieldWalk;

    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, .75f}, // Shield costs 25%
    };
}
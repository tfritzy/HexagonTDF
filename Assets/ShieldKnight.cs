using System.Collections.Generic;

public class ShieldKnight : Enemy
{
    public override EnemyType Type => EnemyType.ShieldKnight;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override float BaseCooldown => AttackSpeed.Slow;
    public override int BaseDamage => 1;
    protected override AnimationState AttackAnimation => AnimationState.ShieldAttack;
    public override int BaseRange => 1;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override float BasePower => 5;
    protected override AnimationState WalkAnimation => AnimationState.ShieldWalk;
    protected override AnimationState IdleAnimation => AnimationState.ShieldIdle;
    public override bool IsMelee => true;

    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        // Shield .25f
        {AttributeType.Health, 1f},
        {AttributeType.MovementSpeed, -.25f} // Movement speed
    };
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickGuy : Enemy
{
    public override EnemyType Type => EnemyType.StickGuy;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override float BaseCooldown => AttackSpeed.Medium;
    public override int BaseDamage => 1;
    protected override AnimationState AttackAnimation => AnimationState.SlashingSword;
    public override float BaseRange => 1;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override float BasePower => 1;
    public override bool IsMelee => true;

    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, 1f},
    };
}

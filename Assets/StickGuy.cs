using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickGuy : Enemy
{
    public override EnemyType Type => EnemyType.StickGuy;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override float Cooldown => AttackSpeed.Medium;
    public override int Damage => 1;
    protected override AnimationState AttackAnimation => AnimationState.SlashingSword;
    public override int Range => 1;
    public override VerticalRegion AttackRegion => throw new System.NotImplementedException();

    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, 1f},
    };
}

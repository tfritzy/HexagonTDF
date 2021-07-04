using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellcaster : Enemy
{
    public override EnemyType Type => EnemyType.Spellcaster;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override float Cooldown => AttackSpeed.Slow;
    public override int Damage => 3;
    public override int Range => 2;
    protected override AnimationState AttackAnimation => AnimationState.CastingSpell;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;

    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, 1f}, // Reducing total power because 
    };
}

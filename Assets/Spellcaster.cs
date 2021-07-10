﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellcaster : Enemy
{
    public override EnemyType Type => EnemyType.Spellcaster;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override float Cooldown => AttackSpeed.Slow;
    public override int BaseDamage => 3;
    public override int BaseRange => 2;
    protected override AnimationState AttackAnimation => AnimationState.CastingSpell;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
    public override float BasePower => 3;

    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, 1f}, // Reducing total power because 
    };
}

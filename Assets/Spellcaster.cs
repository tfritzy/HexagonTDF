﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellcaster : Enemy
{
    public override EnemyType Type => EnemyType.Spellcaster;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    public override VerticalRegion Region => VerticalRegion.Ground;
    protected override float Cooldown => AttackSpeed.Slow;
    protected override int AttackDamage => 3;
    protected override float AttackRange => RangeOptions.Long;
    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, .8f}, // Reducing total power because 
    };
}

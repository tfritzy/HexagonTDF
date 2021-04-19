﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickGuy : Enemy
{
    public override EnemyType Type => EnemyType.StickGuy;

    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;

    public override VerticalRegion Region => VerticalRegion.Ground;
    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, 1f},
    };
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velociraptor : Enemy
{
    public override EnemyType Type => EnemyType.Velociraptor;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    public override float BasePower => 2;
    public override bool IsMelee => true;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override float BaseCooldown => AttackSpeed.Fast;
    public override int BaseDamage => 1;
    public override float BaseRange => 1;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    protected override float BaseMovementSpeed => 2f;

    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, .5f},
        {AttributeType.MovementSpeed, .5f},
    };
}

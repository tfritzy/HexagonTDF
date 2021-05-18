using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDamageEffect : Effect
{
    private const float duration = 5f;
    public override bool Stacks => true;
    public override EffectType Type => EffectType.FireDamage;
    private int Damage;

    public FireDamageEffect(int damage, float timeBetweenTicks, Guid id)
        : base(timeBetweenTicks, duration, id)
    {
        this.Damage = damage;
    }

    protected override void ApplyEffect(Character character)
    {
        character.TakeDamage(this.Damage);
    }
}

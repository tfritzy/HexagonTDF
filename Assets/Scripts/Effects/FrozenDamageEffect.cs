using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenDamageEffect : Effect
{
    private const float duration = 3f;
    public override bool Stacks => true;
    public override EffectType Type => EffectType.FrozenDamage;
    private int Damage;

    public FrozenDamageEffect(int damage, float timeBetweenTicks, Guid id, Character owner)
        : base(timeBetweenTicks, duration, id, owner)
    {
        this.Damage = damage;
    }

    protected override void ApplyEffect(Character character)
    {
        character.TakeDamage(this.Damage, this.Owner);
    }
}

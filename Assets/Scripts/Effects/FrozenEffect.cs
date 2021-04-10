using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenEffect : Effect
{
    public float SlowAmount;
    private const float duration = 3f;
    public override bool Stacks => false;
    public override EffectType Type => EffectType.Frozen;

    public FrozenEffect(float slowAmount, Guid id)
        : base(duration, duration, id)
    {
        this.SlowAmount = slowAmount;
    }

    public override void RemoveEffect(Character character)
    {
        base.RemoveEffect(character);
        if (character is Enemy)
        {
            ((Enemy)character).MovementSpeedModification -= SlowAmount;
            character.SetMaterial(Constants.Materials.Normal);
        }
    }

    protected override void ApplyEffect(Character character)
    {
        if (character is Enemy)
        {
            ((Enemy)character).MovementSpeedModification += SlowAmount;
            character.SetMaterial(Constants.Materials.Frozen);
        }
    }
}

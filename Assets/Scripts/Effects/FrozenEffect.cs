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
    private float modificationAmount;

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
            Enemy enemy = (Enemy)character;
            enemy.MovementSpeedModification += modificationAmount;
            character.SetMaterial(Constants.Materials.Normal);
        }
    }

    protected override void ApplyEffect(Character character)
    {
        if (character is Enemy)
        {
            Enemy enemy = (Enemy)character;
            modificationAmount = (enemy.MovementSpeed - enemy.MovementSpeedModification) * SlowAmount;
            ((Enemy)character).MovementSpeedModification -= modificationAmount;
            character.SetMaterial(Constants.Materials.Frozen);
        }
    }
}

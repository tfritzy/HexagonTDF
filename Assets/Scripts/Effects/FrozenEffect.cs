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

    public FrozenEffect(float slowAmount, Guid id, Character owner)
        : base(duration, duration, id, owner)
    {
        this.SlowAmount = slowAmount;
    }

    public override void RemoveEffect(Character character)
    {
        base.RemoveEffect(character);
        if (character.Alliance == this.Owner.Enemies && character.MovementCell != null)
        {
            character.MovementCell.MovementSpeedModification += modificationAmount;
            character.SetMaterial(Constants.Materials.Normal);
        }
    }

    protected override void ApplyEffect(Character character)
    {
        if (character.Alliance == this.Owner.Enemies && character.MovementCell != null)
        {
            modificationAmount = (character.MovementCell.MovementSpeed - character.MovementCell.MovementSpeedModification) * SlowAmount;
            character.MovementCell.MovementSpeedModification -= modificationAmount;
            character.SetMaterial(Constants.Materials.Frozen);
        }
    }
}

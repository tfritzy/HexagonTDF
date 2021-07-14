using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordThrow : CharacterTargetAbility
{
    public override float Cooldown => 5;

    public SwordThrow(Hero owner) : base(owner) { }

    protected override void Execute()
    {
        this.Target.TakeDamage(100, Owner);
    }

    protected override bool IsValidTarget(Character character)
    {
        return base.IsValidTarget(character) && character is Unit;
    }
}

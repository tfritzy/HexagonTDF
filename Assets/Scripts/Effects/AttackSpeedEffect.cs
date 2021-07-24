using System;

public class AttackSpeedEffect : Effect
{
    public override bool Stacks => false;
    public override EffectType Type => EffectType.AttackSpeed;
    private float percentIncrease;
    private float modificationAmount;

    public AttackSpeedEffect(float percentIncrease, float duration, Guid id, Character owner)
        : base(duration, duration, id, owner)
    {
        this.percentIncrease = percentIncrease;
    }

    protected override void ApplyEffect(Character character)
    {
        modificationAmount = character.Cooldown * percentIncrease;
        character.CooldownModificationAmount -= modificationAmount;
    }

    public override void RemoveEffect(Character character)
    {
        character.CooldownModificationAmount += modificationAmount;
    }
}
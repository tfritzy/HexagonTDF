using System;

public class AttackSpeedEffect : Effect
{
    public override bool Stacks => false;
    public override EffectType Type => EffectType.AttackSpeed;
    private float percentIncrease;

    public AttackSpeedEffect(float percentIncrease, float duration, Guid id, Character owner)
        : base(duration, duration, id, owner)
    {
        this.percentIncrease = percentIncrease;
    }

    protected override void ApplyEffect(Character character)
    {
        if (character.AttackCell != null)
        {
            character.AttackCell.AttackSpeedModifiedPercent += percentIncrease;
        }
    }

    public override void RemoveEffect(Character character)
    {
        if (character.AttackCell != null)
        {
            character.AttackCell.AttackSpeedModifiedPercent -= percentIncrease;
        }

        base.RemoveEffect(character);
    }
}
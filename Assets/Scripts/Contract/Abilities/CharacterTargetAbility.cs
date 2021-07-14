using UnityEngine;

public abstract class CharacterTargetAbility : Ability
{
    public Character Target { get; private set; }
    protected bool IsWaitingForTargetSelection;

    private const string WAITING_FOR_TARGET_MESSAGE = "Select a target.";

    public CharacterTargetAbility(Hero owner) : base(owner) { }

    public void InformCharacterWasClicked(Character character)
    {
        if (IsWaitingForTargetSelection)
        {
            if (IsValidTarget(character))
            {
                this.Target = character;
                IsWaitingForTargetSelection = false;
                Cast();
            }
        }
    }

    protected virtual bool IsValidTarget(Character character)
    {
        return character.IsDead == false;
    }

    public override bool Cast()
    {
        if (Time.time < lastCastTime + Cooldown)
        {
            return false;
        }

        if (this.Target == null)
        {
            IsWaitingForTargetSelection = true;
            return false;
        }

        this.Execute();
        lastCastTime = Time.time;
        IsWaitingForTargetSelection = false;
        this.Target = null;
        return true;
    }

    public override string CurrentStatusMessage()
    {
        if (IsWaitingForTargetSelection)
        {
            return WAITING_FOR_TARGET_MESSAGE;
        }

        return null;
    }
}
using UnityEngine;

public abstract class TargetAbility : Ability
{
    public GameObject Target { get; private set; }
    protected bool IsWaitingForTargetSelection;
    protected abstract string WaitingForInputMessage { get; }

    public TargetAbility(Hero owner) : base(owner) { }

    protected abstract bool IsValidTarget(GameObject gameObject);

    public bool InformGameObjectWasClicked(GameObject gameObject)
    {
        if (IsWaitingForTargetSelection)
        {
            if (IsValidTarget(gameObject))
            {
                this.Target = gameObject;
                IsWaitingForTargetSelection = false;
                Cast();
                return true;
            }
        }

        return false;
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
            return WaitingForInputMessage;
        }

        return null;
    }
}
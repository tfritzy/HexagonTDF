using UnityEngine;

public abstract class InstantCastAbility : Ability
{
    public InstantCastAbility(Hero owner) : base(owner)
    {

    }

    public override bool Cast()
    {
        if (Time.time > lastCastTime + Cooldown)
        {
            lastCastTime = Time.time;
            Execute();
            return true;
        }

        return false;
    }

    public override string CurrentStatusMessage()
    {
        return null;
    }
}
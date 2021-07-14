using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    public abstract float Cooldown { get; }
    protected float lastCastTime;
    protected Hero Owner;

    public Ability(Hero owner)
    {
        lastCastTime = Time.time;
        this.Owner = owner;
    }

    /// <summary>
    /// The method that casts the ability.
    /// </summary>
    public abstract bool Cast();

    /// <summary>
    /// The method to actually execute the functionality of the ability.
    /// </summary>
    protected abstract void Execute();

    public float RemainingCooldownPercent()
    {
        return Mathf.Min(1, (Time.time - lastCastTime) / Cooldown);
    }

    public float RemainingCooldown()
    {
        return Mathf.Max(lastCastTime + Cooldown - Time.time, 0);
    }

    public abstract string CurrentStatusMessage();
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public float Duration;
    public float TimeBetweenTicks;
    public Guid Id;
    public abstract bool Stacks { get; }
    public abstract EffectType Type { get; }
    private float lastApplyTime;
    private float birthTime;
    private bool complete;

    public Effect(float timeBetweenTicks, float duration, Guid id)
    {
        this.Duration = duration;
        this.TimeBetweenTicks = timeBetweenTicks;
        birthTime = Time.time;
        this.Id = id;
    }

    public void Update(Character character)
    {
        if (complete)
        {
            return;
        }

        if (Time.time > birthTime + Duration)
        {
            complete = true;
            RemoveEffect(character);
            return;
        }

        if (Time.time > lastApplyTime + TimeBetweenTicks)
        {
            ApplyEffect(character);
            lastApplyTime = Time.time;
        }
    }

    public virtual void RemoveEffect(Character character)
    {
        character.RemoveEffect(this);
    }

    protected abstract void ApplyEffect(Character character);

    public Effect ShallowCopy()
    {
        return (Effect)this.MemberwiseClone();
    }

    public void Reset()
    {
        this.birthTime = Time.time;
    }
}

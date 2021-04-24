using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public abstract Alliances Alliance { get; }
    public abstract Alliances Enemies { get; }
    public abstract int StartingHealth { get; }
    public abstract float Power { get; }
    public abstract VerticalRegion Region { get; }
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0)
            {
                Die();
            }
        }
    }
    public Vector3 Position
    {
        get
        {
            return Collider != null ? Collider.bounds.center : this.transform.position;
        }
    }
    protected Dictionary<EffectType, Dictionary<Guid, Effect>> Effects;
    protected Collider Collider;
    private int health;

    void Start()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        this.Collider = this.GetComponent<Collider>();
        this.Health = StartingHealth;
        this.Effects = new Dictionary<EffectType, Dictionary<Guid, Effect>>();
    }

    void Update()
    {
        UpdateLoop();
    }

    protected virtual void UpdateLoop()
    {
        ApplyEffects();
    }

    protected virtual void Die()
    {
        Destroy(this.gameObject);
    }

    public virtual void TakeDamage(int amount)
    {
        this.Health -= amount;
        // DamageNumber damageNumber = Instantiate(
        //     Prefabs.DamageNumber,
        //     this.transform.position,
        //     new Quaternion(),
        //     Managers.Canvas)
        //     .GetComponent<DamageNumber>();
        // damageNumber.SetValue(amount, this.gameObject);
        // damageNumber.transform.localScale *= ((float)amount / (float)StartingHealth);
    }

    private void ApplyEffects()
    {
        foreach (EffectType effectType in Effects.Keys)
        {
            foreach (Guid effectId in Effects[effectType].Keys.ToList())
            {
                Effects[effectType][effectId].Update(this);
            }
        }
    }

    public void AddEffect(Effect effect)
    {
        if (this.Effects.ContainsKey(effect.Type) == false || this.Effects[effect.Type] == null)
        {
            this.Effects[effect.Type] = new Dictionary<Guid, Effect>();
        }

        if (effect.Stacks == false)
        {
            if (Effects[effect.Type]?.Count > 0)
            {
                return;
            }
        }

        if (this.Effects[effect.Type].ContainsKey(effect.Id) == false)
        {
            this.Effects[effect.Type][effect.Id] = effect.ShallowCopy();
        }

        this.Effects[effect.Type][effect.Id].Reset();
    }

    public void RemoveEffect(Effect effect)
    {
        this.Effects[effect.Type].Remove(effect.Id);
    }

    public bool ContainsEffect(EffectType type)
    {
        return this.Effects.ContainsKey(type) && this.Effects[type].Count > 0;
    }

    public void SetMaterial(Material material)
    {
        foreach (MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = material;
        }
    }
}
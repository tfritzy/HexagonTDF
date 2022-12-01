using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public virtual BrainCell BrainCell => null;
    public virtual AttackCell AttackCell => null;
    public abstract LifeCell LifeCell { get; }
    public virtual MovementCell MovementCell => null;
    public virtual ConveyorCell ConveyorCell => null;
    public virtual ResourceCollectionCell ResourceCollectionCell => null;
    public virtual ResourceProcessingCell ResourceProcessingCell => null;
    public Vector2Int GridPosition { get; set; }
    public Transform Body;
    public abstract Alliance Enemies { get; }
    public abstract Alliance Alliance { get; }
    public abstract string Name { get; }
    protected Dictionary<EffectType, Dictionary<Guid, Effect>> Effects;
    protected Collider Collider;
    private Rigidbody rb;
    private List<Cell> Cells;

    public Vector3 Position
    {
        get
        {
            return Collider != null ? Collider.bounds.center : this.transform.position;
        }
    }

    protected Rigidbody Rigidbody
    {
        get
        {
            if (rb == null)
            {
                rb = this.GetComponent<Rigidbody>();
            }
            return rb;
        }
    }

    void Start()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        this.Collider = this.GetComponent<Collider>();
        this.Effects = new Dictionary<EffectType, Dictionary<Guid, Effect>>();
        this.Body = this.transform.Find("Body");

        this.Cells = new List<Cell>()
        {
            this.BrainCell,
            this.AttackCell,
            this.LifeCell,
            this.MovementCell,
            this.ResourceCollectionCell,
            this.ConveyorCell,
            this.ResourceProcessingCell,
        };
        this.Cells.RemoveAll((Cell cell) => cell == null);
        this.Cells.ForEach((Cell cell) => cell.Setup(this));
    }

    void Update()
    {
        UpdateLoop();
    }

    protected virtual void UpdateLoop()
    {
        ApplyEffects();
        foreach (Cell cell in this.Cells)
        {
            cell.Update();
        }
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

    public void DisableAllCells()
    {
        this.Cells.ForEach((Cell cell) => cell.SetEnabled(false));
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    // TODO: Add a GetCell<T> so we can stop this madness
    public virtual BrainCell BrainCell => null;
    public virtual AttackCell AttackCell => null;
    public abstract LifeCell LifeCell { get; }
    public virtual MovementCell MovementCell => null;
    public virtual ConveyorCell ConveyorCell => null;
    public virtual ResourceCollectionCell ResourceCollectionCell => null;
    public virtual ResourceProcessingCell ResourceProcessingCell => null;
    public virtual ItemPickupCell ItemPickupCell => null;
    public virtual InventoryCell InventoryCell => null;
    public bool Disabled;

    public Vector2Int GridPosition { get; set; }
    public Transform Body;
    public abstract Alliance Enemies { get; }
    public abstract Alliance Alliance { get; }
    public abstract string Name { get; }
    public Animator Animator;
    protected Dictionary<EffectType, Dictionary<Guid, Effect>> Effects;
    private List<Cell> Cells;
    private CapsuleCollider _capsule;

    public CapsuleCollider Capsule
    {
        get
        {
            if (_capsule == null)
            {
                _capsule = this.GetComponent<CapsuleCollider>();
            }

            return _capsule;
        }
    }

    private Transform _projectileStartPos;
    public Transform ProjectileStartPos
    {
        get
        {
            if (_projectileStartPos == null)
            {
                _projectileStartPos = this.transform.Find("ProjectileStartPosition");
            }

            return _projectileStartPos;
        }
    }

    void Start()
    {
        Setup();
    }

    public virtual void Setup()
    {
        this.Effects = new Dictionary<EffectType, Dictionary<Guid, Effect>>();
        this.Body = this.transform.Find("Body");
        this.GridPosition = Helpers.ToGridPosition(this.transform.position);
        this.Animator = this.Body.GetComponent<Animator>();

        this.Cells = new List<Cell>()
        {
            this.BrainCell,
            this.AttackCell,
            this.LifeCell,
            this.MovementCell,
            this.ResourceCollectionCell,
            this.ConveyorCell,
            this.ResourceProcessingCell,
            this.ItemPickupCell,
            this.InventoryCell,
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
        if (this.Disabled)
        {
            return;
        }

        ApplyEffects();
        foreach (Cell cell in this.Cells)
        {
            if (!cell.IsEnabled)
            {
                continue;
            }

            cell.Update();
        }
    }

    // The event called when the user clicks on a hex, while having this character selected.
    public virtual void SelectedClickHex(HexagonMono hex) { }

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
        foreach (MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>(includeInactive: true))
        {
            renderer.material = material;
        }
    }

    public virtual void SetDefaultMaterial()
    {
        SetMaterial(Prefabs.GetMaterial(MaterialType.ColorPalette));
    }

    public void DisableAllCells()
    {
        this.Cells.ForEach((Cell cell) => cell.SetEnabled(false));
    }

    public virtual void OnDestroy()
    {
        foreach (Cell cell in Cells)
        {
            cell.OnDestroy();
        }
    }
}
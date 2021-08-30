using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour, Damageable
{
    public virtual Vector3 Velocity { get { return Vector3.zero; } }
    public virtual int Damage => BaseDamage;
    public virtual float Range => BaseRange;
    public Vector2Int GridPosition { get; set; }
    public Transform Body;
    public GameObject Projectile;
    public float Cooldown => BaseCooldown / CooldownModificationAmount;
    public float AttackSpeedModifiedPercent;
    public bool IsDead { get; protected set; }
    public AttackPhase AttackPhase;
    public AnimationState _animationState;
    public abstract VerticalRegion Region { get; }
    public abstract VerticalRegion AttackRegion { get; }
    public abstract int StartingHealth { get; }
    public abstract int BaseDamage { get; }
    public abstract float Power { get; }
    public abstract float BaseRange { get; }
    public abstract float BaseCooldown { get; }
    public abstract bool IsMelee { get; }
    public abstract Alliances Enemies { get; }
    public abstract Alliances Alliance { get; }
    protected virtual float ProjectileSpeed => 10;
    protected virtual float CooldownModificationAmount => 1 + AttackSpeedModifiedPercent;
    protected virtual AnimationState WalkAnimation => AnimationState.Walking;
    protected virtual AnimationState IdleAnimation => AnimationState.Idle;
    protected virtual AnimationState AttackAnimation => AnimationState.GeneralAttack;
    protected Transform projectileStartPosition;
    protected Dictionary<EffectType, Dictionary<Guid, Effect>> Effects;
    protected Collider Collider;
    protected Character TargetCharacter;
    private Rigidbody rb;
    private List<TakeDamageTiming> damageTimings;
    private int health;
    private Healthbar healthbar;
    private float lastAttackTime;
    private const float PERCENT_DAMAGE_INCREASE_BY_DOWNHILL_SHOT = .5f;
    private Animator animator;

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

    public AnimationState CurrentAnimation
    {
        get
        {
            return _animationState;
        }
        set
        {
            _animationState = value;

            if (this.animator == null)
            {
                return;
            }

            this.animator.SetInteger("Animation_State", (int)_animationState);
        }
    }

    private class TakeDamageTiming
    {
        public float DamageTime;
        public int Damage;
        public Character Source;
    }

    void Start()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        this.Collider = this.GetComponent<Collider>();
        if (StartingHealth == 0)
        {
            throw new Exception("Starting health should not be 0.");
        }

        this.Health = StartingHealth;
        this.Effects = new Dictionary<EffectType, Dictionary<Guid, Effect>>();
        this.Body = this.transform.Find("Body");
        this.projectileStartPosition = Helpers.RecursiveFindChild(this.transform, "ProjectileStartPosition") ?? this.transform;
        if (this.healthbar == null)
        {
            this.healthbar = Instantiate(Prefabs.Healthbar,
                        new Vector3(10000, 10000),
                        new Quaternion(),
                        Managers.Canvas).GetComponent<Healthbar>();
            this.healthbar.SetOwner(this.transform);
            this.healthbar.enabled = false;
        }
        damageTimings = new List<TakeDamageTiming>();
        FindTargetCharacter();
        this.animator = this.Body.GetComponent<Animator>();
        this.CurrentAnimation = IdleAnimation;
    }

    void Update()
    {
        UpdateLoop();
    }

    protected virtual void UpdateLoop()
    {
        if (IsDead)
        {
            return;
        }

        ApplyEffects();
        ProcessTakeDamageTimings();

        if (TargetCharacter == null)
        {
            this.TargetCharacter = FindTargetCharacter();
        }

        AttackTarget();
    }

    protected abstract Character FindTargetCharacter();

    private void ProcessTakeDamageTimings()
    {
        while (this.damageTimings.Count > 0 && Time.time > this.damageTimings.Last().DamageTime)
        {
            this.TakeDamage(this.damageTimings.Last().Damage, this.damageTimings.Last().Source);
            this.damageTimings.RemoveAt(this.damageTimings.Count - 1);
        }
    }

    protected virtual void Die()
    {
        this.IsDead = true;
        Destroy(this.gameObject);
    }

    public virtual void TakeDamage(int amount, Character source)
    {
        float damageMultiplier = 1;
        if (source != null)
        {
            float heightDifference = source.transform.position.y - this.transform.position.y;
            damageMultiplier = 1 + (heightDifference > 0 ? heightDifference * PERCENT_DAMAGE_INCREASE_BY_DOWNHILL_SHOT : 0);
        }

        this.Health -= (int)((float)amount * damageMultiplier);

        this.healthbar.enabled = true;
        this.healthbar.SetFillScale((float)this.Health / (float)this.StartingHealth);
    }

    public void TakeDamage(int amount, Character source, float delay)
    {
        this.damageTimings.Add(new TakeDamageTiming
        {
            Damage = amount,
            DamageTime = Time.time + delay,
            Source = source,
        });

        this.damageTimings.Sort((x, y) => y.DamageTime.CompareTo(x.DamageTime));
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

    protected virtual void DealDamageToEnemy(Character attacker, Character target)
    {
        target.TakeDamage(this.Damage, this);
    }

    protected virtual bool IsCollisionTarget(Character attacker, GameObject other)
    {
        if (InterfaceUtility.TryGetInterface<Damageable>(out Damageable damageable, other))
        {
            return attacker.Enemies == damageable.Alliance;
        }

        if (other.CompareTag(Constants.Tags.Hexagon))
        {
            return true;
        }

        return false;
    }

    public bool IsNull()
    {
        return this == null;
    }

    private GameObject windingUpProjectile;
    public void BeginWindup()
    {
        this.AttackPhase = AttackPhase.WindingUp;

        if (IsMelee == false && windingUpProjectile == null)
        {
            windingUpProjectile = Instantiate(
                Projectile,
                this.projectileStartPosition.position,
                new Quaternion(),
                this.projectileStartPosition.transform);
        }
    }

    public void ReleaseAttack()
    {
        this.AttackPhase = AttackPhase.Recovering;

        if (!IsInRangeOfTarget())
        {
            return;
        }

        if (IsMelee)
        {
            this.DealDamageToEnemy(this, this.TargetCharacter);
        }
        else
        {
            ConfigureProjectile(windingUpProjectile);
            windingUpProjectile.transform.parent = null;
            windingUpProjectile = null;
        }
    }

    protected virtual bool IsInRangeOfTarget()
    {
        if (TargetCharacter == null)
        {
            return false;
        }

        return (TargetCharacter.transform.position - this.transform.position).magnitude <= this.Range;
    }

    public void FinishedRecovering()
    {
        this.AttackPhase = AttackPhase.Idle;
        this.CurrentAnimation = IdleAnimation;
    }

    protected virtual void ConfigureProjectile(GameObject projectile)
    {
        projectile.transform.parent = null;
        if (projectile.TryGetComponent<Projectile>(out Projectile projectileMono))
        {
            projectileMono.Initialize(DealDamageToEnemy, IsCollisionTarget, this);
            projectileMono.SetTracking(this.TargetCharacter.gameObject, this.ProjectileSpeed);
        }
    }

    protected void AttackTarget()
    {
        if (Time.time > lastAttackTime + this.Cooldown && IsInRangeOfTarget() && AttackPhase == AttackPhase.Idle)
        {
            if (this.animator == null)
            {
                BeginWindup();
                ReleaseAttack();
                FinishedRecovering();
            }
            else
            {
                this.AttackPhase = AttackPhase.WindingUp;
                this.CurrentAnimation = this.AttackAnimation;
                if (this.Rigidbody != null) this.Rigidbody.velocity = Vector3.zero;
                Vector3 diffVector = TargetCharacter.transform.position - this.transform.position;
                diffVector.y = 0;
                this.transform.rotation = Quaternion.LookRotation(diffVector, Vector3.up);
            }

            lastAttackTime = Time.time;
        }
    }

}
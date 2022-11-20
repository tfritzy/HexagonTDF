using UnityEngine;

public abstract class AttackCell : Cell
{
    public abstract float BaseCooldown { get; }
    public abstract int BaseDamage { get; }
    public abstract float BaseRange { get; }
    public abstract VerticalRegion AttackRegion { get; }
    public abstract VerticalRegion Region { get; }
    public abstract bool InstantaneousAttacks { get; }

    public float Cooldown => BaseCooldown / CooldownModificationAmount;

    public virtual int Damage => BaseDamage;
    public virtual float Range => BaseRange;
    protected virtual float ExplosionRadius => 0;
    protected virtual bool DoProjectilesTrack => false;
    protected virtual float ProjectileSpeed => 10;
    protected virtual int MaxPierceCount => 0;
    protected virtual float CooldownModificationAmount => 1 + AttackSpeedModifiedPercent;

    public float AttackSpeedModifiedPercent;
    public GameObject Projectile;
    private float lastAttackTime;
    protected Transform projectileStartPosition;
    private GameObject windingUpProjectile;

    public override void Setup(Character character)
    {
        base.Setup(character);

        this.projectileStartPosition =
            Helpers.RecursiveFindChild(
                this.Owner.Body,
                "ProjectileStartPosition")
            ?? this.Owner.Body;
    }


    private void Explode(Character attacker, Character target)
    {
        Collider[] nearby = Physics.OverlapSphere(
            target.transform.position,
            this.ExplosionRadius,
            Constants.Layers.Characters,
            QueryTriggerInteraction.Collide);
        foreach (Collider collider in nearby)
        {
            if (collider.TryGetComponent<Character>(out Character character))
            {
                if (character.LifeCell != null && attacker.Enemies == character.Alliance)
                {
                    character.LifeCell.TakeDamage(Damage, this.Owner);
                }
            }
        }
    }

    protected virtual void ConfigureProjectile(GameObject projectile, Character target)
    {
        projectile.transform.parent = null;
        if (projectile.TryGetComponent<Projectile>(out Projectile projectileMono))
        {
            projectileMono.Initialize(
                DealDamageToEnemy,
                IsCollisionTarget,
                this.Owner,
                this.MaxPierceCount);

            if (DoProjectilesTrack && target != null)
            {
                projectileMono.SetTracking(target.gameObject, this.ProjectileSpeed);
            }
        }
    }

    protected virtual void DealDamageToEnemy(Character attacker, Character target)
    {
        if (this.ExplosionRadius == 0)
        {
            target.LifeCell?.TakeDamage(this.Damage, this.Owner);
        }
        else
        {
            this.Explode(this.Owner, target);
        }
    }


    public void ReleaseAttack(Character target)
    {
        if (!IsInRangeOfTarget(target))
        {
            return;
        }

        if (this.InstantaneousAttacks)
        {
            this.DealDamageToEnemy(this.Owner, target);
        }
        else
        {
            ConfigureProjectile(windingUpProjectile, target);
            windingUpProjectile.transform.parent = null;
            windingUpProjectile = null;
        }
    }

    protected virtual bool IsInRangeOfTarget(Character target)
    {
        if (target == null)
        {
            return false;
        }

        // TODO use distance checking system.
        Vector3 distToTarget =
            target.transform.position - this.Owner.Position;
        return distToTarget.magnitude <= this.Owner.AttackCell.Range;
    }

    private bool IsCollisionTarget(Character attacker, GameObject collision)
    {
        if (collision == null)
        {
            return false;
        }

        if (!collision.TryGetComponent<Character>(out Character character))
        {
            return false;
        }

        if (character.LifeCell == null)
        {
            return false;
        }

        return character.Alliance == this.Owner.Enemies;
    }
}
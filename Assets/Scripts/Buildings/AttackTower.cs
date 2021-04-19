using UnityEngine;

public abstract class AttackTower : Building
{
    public abstract float Cooldown { get; }
    public abstract int Damage { get; }
    public abstract float Range { get; }
    public abstract VerticalRegion AttackRegion { get; }
    public virtual int NumProjectiles => 1;
    public virtual float ProjectileStartPostionRandomness => 0f;
    protected virtual float ManualPowerAdjustment => 0;
    protected virtual int ExpectedNumberOfEnemiesHitByEachProjectile => 1;
    protected virtual float ExplosionRadius => 0;
    public Character Target;
    protected const float projectileSpeed = 10;
    protected Vector3 projectileStartPosition;
    protected GameObject Turret;
    protected GameObject Body;

    protected override void Setup()
    {
        this.projectileStartPosition = this.transform.Find("ProjectileStartPosition")?.transform.position ?? this.transform.position;
        this.Turret = transform.Find("Body")?.Find("Turret")?.gameObject;
        this.Body = transform.Find("Body")?.gameObject;
        base.Setup();
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();
        CheckForTarget();
        LookAtTarget();
        AttackIfPossible();
    }

    protected float lastAttackTime;
    private void AttackIfPossible()
    {
        if (CanAttack())
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    protected virtual bool CanAttack()
    {
        return Time.time > lastAttackTime + Cooldown && Target != null;
    }

    protected virtual void LookAtTarget()
    {
        if (this.Turret == null || this.Target == null)
        {
            return;
        }

        Vector3 targetPos = Target.transform.position;
        targetPos.y = Turret.transform.position.y;
        Turret.transform.LookAt(targetPos, Vector3.up);
    }

    protected virtual void Attack()
    {
        for (int i = 0; i < NumProjectiles; i++)
        {
            GameObject projectile = Instantiate(
                Prefabs.Projectiles[Type],
                this.projectileStartPosition,
                new Quaternion());
            projectile.GetComponent<Projectile>().Initialize(DealDamageToEnemy, this);
            projectile.transform.LookAt(this.Target.transform, Vector3.up);
            SetProjectileVelocity(projectile);

            // Want first projectile to be perfectly accurate.
            if (i > 0)
            {
                projectile.transform.position = projectile.transform.position + Random.insideUnitSphere * ProjectileStartPostionRandomness;
            }
        }
    }

    protected float timeBetweenTargetChecks = .5f;
    protected float lastTargetCheckTime;
    protected virtual void CheckForTarget()
    {
        if (Time.time + lastTargetCheckTime > timeBetweenTargetChecks)
        {
            this.Target = this.FindTarget();
            lastTargetCheckTime = Time.time;
        }
    }

    protected Character FindTarget()
    {
        Collider[] nearby = Physics.OverlapSphere(this.transform.position, Range, Constants.Layers.Characters, QueryTriggerInteraction.Collide);
        Character closest = null;
        float closestDist = float.MaxValue;
        foreach (Collider collider in nearby)
        {
            Character character = collider.transform?.GetComponent<Character>();
            if (character == null)
            {
                continue;
            }

            if (this.Enemies == character.Alliance)
            {
                float distance = Vector3.Distance(collider.transform.position, this.transform.position);
                if (distance < closestDist)
                {
                    closest = character;
                    closestDist = distance;
                }
            }
        }

        return closest;
    }

    protected void DealDamageToEnemy(Character attacker, Character target, GameObject projectile)
    {
        // For cases where the projectile hits the ground and such.
        if (target == null)
        {
            return;
        }

        if (ExplosionRadius == 0)
        {
            target.TakeDamage(Damage);
        }
        else
        {
            Explode(attacker, target, projectile);
        }
    }

    protected void SetProjectileVelocity(GameObject projectile)
    {
        float flightDuration = (Target.transform.position - projectile.transform.position).magnitude / projectileSpeed;
        Vector3 targetPosition = Target.transform.position + Target.GetComponent<Rigidbody>().velocity * flightDuration;
        projectile.GetComponent<Rigidbody>().velocity = (targetPosition - projectile.transform.position).normalized * projectileSpeed;
    }

    public void CreateRangeCircle(Transform parent)
    {
        Destroy(parent.Find("Range Circle")?.gameObject);
        GameObject circle = Instantiate(Prefabs.RangeCircle, parent.position, Prefabs.RangeCircle.transform.rotation, parent);
        circle.name = "Range Circle";
        circle.transform.localScale *= Range;
    }

    public override float Power
    {
        get
        {
            return getRangePower() + getDamagePower() + getCooldownPower() + getAttackRegionPower() + ManualPowerAdjustment + getPopulationPower();
        }
    }

    private float getPopulationPower()
    {
        return -((float)PopulationCost) / Constants.ResourcePowerMap[ResourceType.Population];
    }

    private float getDamagePower()
    {
        return (Damage * NumProjectiles * ExpectedNumberOfEnemiesHitByEachProjectile) / 10;
    }

    private void Explode(Character attacker, Character target, GameObject projectile)
    {
        Collider[] nearby = Physics.OverlapSphere(projectile.transform.position, this.ExplosionRadius, Constants.Layers.Characters, QueryTriggerInteraction.Collide);
        foreach (Collider collider in nearby)
        {
            Character character = collider.transform?.GetComponent<Character>();
            if (character == null)
            {
                continue;
            }

            if (attacker.Enemies == target.Alliance)
            {
                character.TakeDamage(Damage);
            }
        }
    }

    private float getRangePower()
    {
        switch (Range)
        {
            case (RangeOptions.Short):
                return 0;
            case (RangeOptions.Medium):
                return 1;
            case (RangeOptions.Long):
                return 2;
            case (RangeOptions.VeryLong):
                return 3;
            default:
                throw new System.ArgumentException($"Range {Range} isn't a prescribed option");
        }
    }

    private float getCooldownPower()
    {
        switch (Cooldown)
        {
            case (AttackSpeed.Slow):
                return -1;
            case (AttackSpeed.Medium):
                return 0;
            case (AttackSpeed.Fast):
                return 1;
            case (AttackSpeed.VeryFast):
                return 2;
            default:
                throw new System.ArgumentException($"Cooldown should be a value in AttackSpeed. {Cooldown} is not.");
        }
    }

    private float getAttackRegionPower()
    {
        switch (AttackRegion)
        {
            case (VerticalRegion.Air):
                return 0;
            case (VerticalRegion.Ground):
                return 0;
            case (VerticalRegion.GroundAndAir):
                return 1;
            default:
                throw new System.ArgumentException($"Vertical Regions {AttackRegion} not an expected option");
        }
    }
}
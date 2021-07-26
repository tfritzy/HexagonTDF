using UnityEngine;

public abstract class AttackTower : Building, Interactable
{
    public virtual int NumProjectiles => 1;
    public virtual float ProjectileStartPostionRandomness => 0f;
    public Character Target;
    protected virtual float ManualPowerAdjustment => 0;
    protected virtual int ExpectedNumberOfEnemiesHitByEachProjectile => 1;
    protected virtual float ExplosionRadius => 0;
    protected GameObject Turret;
    protected GameObject Body;
    public override int StartingHealth => 15; // TODO: Set appropriate value per tower.
    public override int Damage => GetDamage(UpgradeLevel);
    public override float BaseRange => GetRange(UpgradeLevel);
    public override float Power => GetPower(UpgradeLevel);
    protected override float CooldownModificationAmount => GetCooldownModificationAmount(UpgradeLevel);
    public ResourceTransaction UpgradeCost { get; private set; }

    public int UpgradeLevel;

    protected override void Setup()
    {
        this.Turret = transform.Find("Turret")?.gameObject;
        this.Body = transform.Find("Body")?.gameObject;
        UpgradeLevel = 0;
        UpgradeCost = new ResourceTransaction(GetUpgradeCost());
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

    private int GetDamage(int upgradeLevel)
    {
        return (int)(this.BaseDamage * (1 + .5f * upgradeLevel));
    }

    private int GetRange(int upgradeLevel)
    {
        return (int)(this.BaseRange * (1 + .1f * upgradeLevel));
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

        Vector3 targetPos = Target.Position;
        targetPos.y = Turret.transform.position.y;
        Turret.transform.LookAt(targetPos, Vector3.up);
    }

    protected virtual void Attack()
    {
        for (int i = 0; i < NumProjectiles; i++)
        {
            GameObject projectile = Instantiate(
                Prefabs.Projectiles[Type],
                this.projectileStartPosition.position,
                new Quaternion());
            projectile.GetComponent<Projectile>().Initialize(DealDamageToEnemy, IsCollisionTarget, this);
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

    protected virtual Character FindTarget()
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

            if (character is Building)
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

    protected override void DealDamageToEnemy(Character attacker, Character target, GameObject projectile)
    {
        // target can be null on contact with ground.
        if (ExplosionRadius == 0 && target != null)
        {
            target.TakeDamage(Damage, this);
        }
        else
        {
            Explode(attacker, target, projectile);
        }
    }

    protected void SetProjectileVelocity(GameObject projectile)
    {
        float flightDuration = (Target.Position - projectile.transform.position).magnitude / ProjectileSpeed;
        Vector3 targetPosition = Target.Position + Target.Velocity * flightDuration;
        projectile.GetComponent<Rigidbody>().velocity = (targetPosition - projectile.transform.position).normalized * ProjectileSpeed;
    }

    public void CreateRangeCircle(Transform parent)
    {
        Destroy(parent.Find("Range Circle")?.gameObject);
        GameObject circle = Instantiate(Prefabs.RangeCircle, parent.position, Prefabs.RangeCircle.transform.rotation, parent);
        circle.name = "Range Circle";
        circle.transform.localScale *= Range;
    }

    public void Upgrade()
    {
        if (this.UpgradeCost.CanFulfill())
        {
            this.UpgradeCost.Deduct();
            this.UpgradeLevel += 1;
            this.UpgradeCost = new ResourceTransaction(this.GetUpgradeCost());
            Debug.Log("Upgraded tower to level " + UpgradeLevel);
        }
        else
        {
            Debug.Log("Not enough gold for upgrade");
        }
    }

    private int GetUpgradeCost()
    {
        float powerIncrease = GetPower(UpgradeLevel + 1) - GetPower(UpgradeLevel);
        return (int)(powerIncrease * Constants.ResourcePowerMap[ResourceType.Gold]);
    }

    private float GetPower(int upgradeLevel)
    {
        float power = getDamagePower(upgradeLevel) * getAttackRegionMultiplier() * getRangePowerMultiplier(upgradeLevel) + ManualPowerAdjustment;
        Debug.Log($"{this.Type} Power\n {getDamagePower(upgradeLevel)} damage * {getAttackRegionMultiplier()} region * {getRangePowerMultiplier(upgradeLevel)} range + {ManualPowerAdjustment} manual == {power}");
        return power;
    }

    private float GetCooldown(int upgradeLevel)
    {
        return Cooldown / GetCooldownModificationAmount(upgradeLevel);
    }

    private float GetCooldownModificationAmount(int upgradeLevel)
    {
        return base.CooldownModificationAmount + upgradeLevel * .25f;
    }

    private float getDamagePower(int upgradeLevel)
    {
        float dps = ((GetDamage(upgradeLevel) * NumProjectiles * ExpectedNumberOfEnemiesHitByEachProjectile) / GetCooldown(upgradeLevel));
        return dps / Constants.ENEMY_HEALTH_PER_POWER;
    }

    private void Explode(Character attacker, Character target, GameObject projectile)
    {
        Collider[] nearby = Physics.OverlapSphere(projectile.transform.position, this.ExplosionRadius, Constants.Layers.Characters, QueryTriggerInteraction.Collide);
        foreach (Collider collider in nearby)
        {
            if (InterfaceUtility.TryGetInterface<Damageable>(out Damageable damageable, collider.gameObject))
            {
                continue;
            }

            if (attacker.Enemies == damageable.Alliance)
            {
                damageable.TakeDamage(Damage, this);
            }
        }
    }

    private float getRangePowerMultiplier(int upgradeLevel)
    {
        return GetRange(upgradeLevel) / 3;
    }

    private float getAttackRegionMultiplier()
    {
        switch (AttackRegion)
        {
            case (VerticalRegion.Air):
                return 1;
            case (VerticalRegion.Ground):
                return 1;
            case (VerticalRegion.GroundAndAir):
                return 1.2f;
            default:
                throw new System.ArgumentException($"Vertical Regions {AttackRegion} not an expected option");
        }
    }

    protected RaycastHit[] ShootRaycastFromTurret(float maxDistance)
    {
        Vector3 targetsPos = Target.GetComponent<Collider>().bounds.center;
        Vector3 source = this.transform.position;
        source.y = targetsPos.y;
        Vector3 direction = targetsPos - this.transform.position;
        direction.y = 0;
        return Physics.RaycastAll(source, direction, maxDistance, Constants.Layers.Characters);
    }

    public bool Interact()
    {
        if (Managers.SelectTowerMenu.gameObject.activeInHierarchy && Managers.SelectTowerMenu.TargetTower == this)
        {
            Managers.SelectTowerMenu.gameObject.SetActive(false);
        }
        else
        {
            Managers.SelectTowerMenu.gameObject.SetActive(true);
            Managers.SelectTowerMenu.SetTargetTower(this);
        }

        return true;
    }
}
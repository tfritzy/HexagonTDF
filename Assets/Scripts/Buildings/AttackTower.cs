using UnityEngine;

public abstract class AttackTower : Building, Interactable
{
    public int UpgradeLevel;
    public float Rotation;
    public ResourceTransaction UpgradeCost { get; private set; }
    public virtual int NumProjectiles => 1;
    public virtual float ProjectileStartPostionRandomness => 0f;
    public override int StartingHealth => 15; // TODO: Set appropriate value per tower.
    public override int Damage => GetDamage(UpgradeLevel);
    public override float Range => GetRange(UpgradeLevel);
    public override float Power => GetPower(UpgradeLevel);
    public override bool IsMelee => false;
    protected virtual float ExpectedNumberOfEnemiesHitByEachProjectile => 1;
    protected virtual float ManualPowerAdjustment => 0;
    protected override float CooldownModificationAmount => GetCooldownModificationAmount(UpgradeLevel);
    protected virtual float RotationVelocityDegreesPerSec => 180;
    protected GameObject Turret;
    private Vector3 rangeCircleOriginalScale;
    private Vector3 FacingDirection => new Vector3(
        Mathf.Cos((Rotation) * Mathf.Deg2Rad),
        0,
        Mathf.Sin((Rotation) * Mathf.Deg2Rad));
    private GameObject rangeCircle;
    private Vector3 turretStartRotation;

    protected override void Setup()
    {
        this.Turret = Helpers.RecursiveFindChild(this.transform, "Turret")?.gameObject;
        this.turretStartRotation = this.Turret?.transform.localRotation.eulerAngles ?? Vector3.zero;
        UpgradeLevel = 0;
        UpgradeCost = new ResourceTransaction(GetUpgradeCost());
        base.Setup();
    }

    protected override void UpdateLoop()
    {
        if (this.IsDead)
        {
            return;
        }

        RotateTowardsTarget();
        base.UpdateLoop();
    }

    private void RotateTowardsTarget()
    {
        if (this.Turret == null || this.TargetCharacter == null)
        {
            return;
        }

        Vector3 diffVector = TargetCharacter.transform.position - this.Turret.transform.position;
        diffVector.y = 0;
        float angleDir = Helpers.AngleDir(FacingDirection, diffVector);
        Rotation += angleDir * RotationVelocityDegreesPerSec * Time.deltaTime;
        Turret.transform.localRotation = Quaternion.Euler(new Vector3(0, Rotation, 0)
        );
    }

    protected override bool CanAttack()
    {
        if (base.CanAttack() == false)
        {
            return false;
        }

        float angleBetween = Helpers.AngleXZ(
            FacingDirection,
            TargetCharacter.transform.position - this.Turret.transform.position);
        print(angleBetween);
        if (angleBetween > 10)
        {
            return false;
        }

        return true;
    }

    protected override bool IsTargetStillValid()
    {
        return (TargetCharacter.transform.position - this.transform.position).magnitude <= this.Range;
    }

    private int GetDamage(int upgradeLevel)
    {
        return (int)(this.BaseDamage * (1 + .25f * upgradeLevel));
    }

    private float GetRange(int upgradeLevel)
    {
        return this.BaseRange * (1 + .2f * upgradeLevel);
    }

    protected override void ConfigureProjectile(GameObject projectile)
    {
        for (int i = 0; i < NumProjectiles; i++)
        {
            if (projectile.TryGetComponent<Projectile>(out Projectile projectileMono))
            {
                base.ConfigureProjectile(projectile);
                SetProjectileVelocity(projectile.gameObject);
                projectile.transform.LookAt(this.TargetCharacter.transform, Vector3.up);
            }

            // Want first projectile to be perfectly accurate.
            if (i > 0)
            {
                projectile.transform.position = projectile.transform.position + Random.insideUnitSphere * ProjectileStartPostionRandomness;
            }
        }
    }

    protected override Character FindTargetCharacter()
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

    protected void SetProjectileVelocity(GameObject projectile)
    {
        float flightDuration = (this.TargetCharacter.Position - projectile.transform.position).magnitude / ProjectileSpeed;
        Vector3 targetPosition = this.TargetCharacter.Position + this.TargetCharacter.Velocity * flightDuration;
        projectile.GetComponent<Rigidbody>().velocity = (targetPosition - projectile.transform.position).normalized * ProjectileSpeed;
    }

    public void ShowRangeCircle(Transform parent = null)
    {
        if (parent == null)
        {
            parent = this.transform;
        }

        if (rangeCircle == null)
        {
            this.rangeCircle = Instantiate(Prefabs.RangeCircle, parent.position, Prefabs.RangeCircle.transform.rotation, parent);
            this.rangeCircle.name = "Range Circle";
            this.rangeCircleOriginalScale = this.rangeCircle.transform.localScale;
        }
        else
        {
            this.rangeCircle.SetActive(true);
        }

        this.rangeCircle.transform.localScale = rangeCircleOriginalScale * Range;
    }

    public void HideRangeCircle()
    {
        this.rangeCircle?.SetActive(false);
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
        float power = (getDamagePower(upgradeLevel) * getAttackRegionMultiplier() * getRangePowerMultiplier(upgradeLevel) + ManualPowerAdjustment) * Constants.HARD_DIFFICULTY_ADJUSTMENT;
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
        return (dps / Constants.ENEMY_HEALTH_PER_POWER) * Constants.BALANCE_INTERVAL_SECONDS;
    }

    private float getRangePowerMultiplier(int upgradeLevel)
    {
        return 1 + (GetRange(upgradeLevel) / RangeOptions.VeryShort) / 10;
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
        Vector3 targetsPos = this.TargetCharacter.GetComponent<Collider>().bounds.center;
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
            Managers.SelectTowerMenu.Disable();
        }
        else
        {
            Managers.SelectTowerMenu.gameObject.SetActive(true);
            Managers.SelectTowerMenu.SetTargetTower(this);
        }

        return true;
    }

    protected override bool IsCollisionTarget(Character attacker, GameObject other)
    {
        if (other.GetComponent<Building>() != null)
        {
            return false;
        }

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
}
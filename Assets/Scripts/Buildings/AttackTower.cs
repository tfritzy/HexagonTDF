using UnityEngine;

public abstract class AttackTower : Building
{
    public abstract float Cooldown { get; }
    public abstract int Damage { get; }
    public abstract float Range { get; }
    public Character Target;
    protected const float projectileSpeed = 10;
    protected Vector3 projectileStartPosition;

    protected override void Setup()
    {
        this.projectileStartPosition = this.transform.Find("ProjectileStartPosition")?.transform.position ?? this.transform.position;
        base.Setup();
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();
        CheckForTarget();
        AttackIfPossible();
    }

    protected float lastAttackTime;
    private void AttackIfPossible()
    {
        if (Time.time > lastAttackTime + Cooldown && Target != null)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    protected virtual void Attack()
    {
        GameObject projectile = Instantiate(Prefabs.Projectiles[Type], this.projectileStartPosition, new Quaternion());
        projectile.GetComponent<Projectile>().Initialize(DealDamageToEnemy, this);
        projectile.transform.LookAt(this.Target.transform, Vector3.up);
        SetProjectileVelocity(projectile);
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

        target.TakeDamage(Damage);
    }

    protected void SetProjectileVelocity(GameObject projectile)
    {
        float flightDuration = (Target.transform.position - projectile.transform.position).magnitude / projectileSpeed;
        Vector3 targetPosition = Target.transform.position + Target.GetComponent<Rigidbody>().velocity * flightDuration;
        projectile.GetComponent<Rigidbody>().velocity = (targetPosition - projectile.transform.position).normalized * projectileSpeed;
    }
}
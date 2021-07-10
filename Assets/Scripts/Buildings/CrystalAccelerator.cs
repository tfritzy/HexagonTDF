using System.Collections.Generic;
using UnityEngine;

public class CrystalAccelerator : AttackTower
{
    public override float Cooldown => AttackSpeed.Slow;
    public override int BaseDamage => 10;
    public override int BaseRange => RangeOptions.Short;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override BuildingType Type => BuildingType.CrystalAccelerator;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    protected override int ExpectedNumberOfEnemiesHitByEachProjectile => 12;
    private ParticleSystem projectileGenerationAnimation;

    protected override void Setup()
    {
        base.Setup();
        this.projectileGenerationAnimation = Body.transform.Find("ProjectileGeneration").GetComponent<ParticleSystem>();
    }

    protected override void Attack()
    {
        HurtEnemies();
        SpawnAnimation();
    }

    private void HurtEnemies()
    {
        foreach (RaycastHit hit in ShootRaycastFromTurret(100f))
        {
            if (InterfaceUtility.TryGetInterface<Damageable>(out Damageable damageable, hit.collider.gameObject))
            {
                if (damageable.Alliance == this.Enemies)
                {
                    damageable.TakeDamage(Damage, this);
                }
            }
        }
    }

    private void SpawnAnimation()
    {
        GameObject projectile = Instantiate(Prefabs.Projectiles[this.Type], Vector3.zero, new Quaternion(), null);
        GameObject smoke = projectile.transform.Find("Smoke").gameObject;
        GameObject particles = projectile.transform.Find("ParticlesOutBack").gameObject;
        particles.transform.rotation = particles.transform.rotation * Turret.transform.rotation;
        particles.transform.position = Turret.transform.position;
        Vector3 targetsPos = Target.GetComponent<Collider>().bounds.center;
        Vector3 vectorToTarget = targetsPos - this.transform.position;
        vectorToTarget.y = 0;
        Vector3 smokePosition = this.transform.position + vectorToTarget.normalized * smoke.transform.position.z;
        smokePosition.y = targetsPos.y;
        smoke.transform.position = smokePosition;
        var main = smoke.GetComponent<ParticleSystem>().main;
        float angle = Vector3.Angle(Vector3.forward, vectorToTarget);
        main.startRotationZ = main.startRotationZ.constant + angle * Mathf.Deg2Rad;
        Destroy(projectile, 5f);
        this.projectileGenerationAnimation.Play();
    }
}
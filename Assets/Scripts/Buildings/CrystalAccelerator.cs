using System.Collections.Generic;
using UnityEngine;

public class CrystalAccelerator : AttackTower
{
    public override float Cooldown => AttackSpeed.Slow;
    public override int Damage => 5;
    public override float Range => RangeOptions.Short;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override BuildingType Type => BuildingType.CrystalAccelerator;
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    protected override int ExpectedNumberOfEnemiesHitByEachProjectile => 8;
    private static Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>()
    {
        {ResourceType.Stone, .7f},
        {ResourceType.Gold, .3f}
    };
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
        Vector3 source = this.transform.position;
        source.y = Target.transform.position.y;
        Vector3 direction = Target.transform.position - this.transform.position;
        direction.y = 0;
        RaycastHit[] hits = Physics.RaycastAll(source, direction, 100f, Constants.Layers.Characters);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent<Character>(out Character character))
            {
                if (character.Alliance == this.Enemies)
                {
                    character.TakeDamage(Damage);
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
        Vector3 vectorToTarget = Target.transform.position - this.transform.position;
        vectorToTarget.y = 0;
        Vector3 smokePosition = this.transform.position + vectorToTarget.normalized * smoke.transform.position.z;
        smokePosition.y = Target.transform.position.y;
        smoke.transform.position = smokePosition;
        var main = smoke.GetComponent<ParticleSystem>().main;
        float angle = Vector3.Angle(Vector3.forward, vectorToTarget);
        main.startRotationZ = main.startRotationZ.constant + angle * Mathf.Deg2Rad;
        Destroy(projectile, 5f);
        this.projectileGenerationAnimation.Play();
    }
}
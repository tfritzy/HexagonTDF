using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerTower : AttackTower
{
    public GameObject Impact;
    public override BuildingType Type => BuildingType.HammerTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Maltov;
    public override float BaseCooldown => AttackSpeed.Slow;
    public override int BaseDamage => 75;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override float BaseRange => RangeOptions.VeryShort;
    public override VerticalRegion Region => VerticalRegion.Ground;
    protected override bool CanProjectilesHitMultipleTargets => true;
    public override bool IsMelee => true;
    protected override float ExplosionRadius => .5f;
    protected override float RotationVelocityDegreesPerSec => 90;
    protected override float ExpectedNumberOfEnemiesHitByEachProjectile => 1.5f;
    private GameObject impact;

    protected override void Setup()
    {
        base.Setup();
        impact = Instantiate(Impact, this.transform);
        Helpers.TriggerAllParticleSystems(impact.transform, false);
    }

    protected override void DealDamageToEnemy(Character attacker, Character target)
    {
        base.DealDamageToEnemy(attacker, target);
        impact.SetActive(true);
        this.impact.transform.position = target.transform.position + Vector3.up * .01f;
        Helpers.TriggerAllParticleSystems(impact.transform, true);
    }
}

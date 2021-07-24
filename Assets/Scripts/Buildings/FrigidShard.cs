using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrigidShard : AttackTower
{
    public override float BaseCooldown => AttackSpeed.Fast;
    public override int BaseDamage => 1;
    public override int BaseRange => RangeOptions.Short;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override BuildingType Type => BuildingType.FrigidShard;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public float SlowAmount => .5f;
    protected override int ExpectedNumberOfEnemiesHitByEachProjectile => 10;
    protected override float ManualPowerAdjustment => 8;
    private FrozenEffect frozenEffect;
    private FrozenDamageEffect frozenDamageEffect;

    protected override void Setup()
    {
        this.GetComponent<SphereCollider>().radius = this.Range;
        this.frozenEffect = new FrozenEffect(this.SlowAmount, Guid.NewGuid(), this);
        this.frozenDamageEffect = new FrozenDamageEffect(this.Damage, this.Cooldown, Guid.NewGuid(), this);
        base.Setup();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
        {
            if (enemy.ContainsEffect(EffectType.Frozen) == false)
            {
                enemy.AddEffect(frozenEffect);
                enemy.AddEffect(frozenDamageEffect);
            }
        }
    }

    protected override void Attack()
    {
        // No need to do anything. All is handled by applied effects.
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerTower : AttackTower
{
    public override float Cooldown => AttackSpeed.VeryVeryVeryFast;
    public override int Damage => 1;
    public override float Range => RangeOptions.Short;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override BuildingType Type => BuildingType.FlamethrowerTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override bool IsVillageBuilding => false;

    private FireDamageEffect effect;
    private ParticleSystem flamethrowerPS;

    // Burning lasts a while after attacking stops, and flame particles fly outside range.
    protected override float ManualPowerAdjustment => 2;

    protected override void Setup()
    {
        base.Setup();
        flamethrowerPS = Turret.transform.Find("Flames").GetComponent<ParticleSystem>();
        flamethrowerPS.Stop();
    }

    protected override Character FindTarget()
    {
        Character target = base.FindTarget();
        if (target == null)
        {
            flamethrowerPS.Stop();
        }
        return target;
    }

    protected override void Attack()
    {
        flamethrowerPS.Play();
        effect = new FireDamageEffect(this.Damage, this.Cooldown, Guid.NewGuid(), this);
    }

    public override void TriggerParticleCollision(GameObject collidedWith)
    {
        if (collidedWith.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.AddEffect(effect);
        }
    }
}
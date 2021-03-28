using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTower : AttackTower
{
    public override BuildingType Type => BuildingType.LightningTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;


    public override float Cooldown => 3;
    public override int Damage => 2;
    public override float Range => 10;

    protected override void Attack()
    {
        GameObject projectile = Instantiate(Prefabs.Projectiles[Type], Target.transform.position, Prefabs.Projectiles[Type].transform.rotation);
        Target.TakeDamage(this.Damage);
    }
}

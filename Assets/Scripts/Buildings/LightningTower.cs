using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTower : AttackTower
{
    public override BuildingType Type => BuildingType.LightningTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override float Cooldown => 5;
    public override int Damage => 10;
    public override float Range => 9f;
    public override ResourceTransaction BuildCost => new ResourceTransaction(stone: 120, wood: 40, gold: 0);

    protected override void Attack()
    {
        GameObject projectile = Instantiate(Prefabs.Projectiles[Type], Target.transform.position, Prefabs.Projectiles[Type].transform.rotation);
        Target.TakeDamage(this.Damage);
    }
}

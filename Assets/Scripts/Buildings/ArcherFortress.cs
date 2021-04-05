using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcherFortress : AttackTower
{
    public override BuildingType Type => BuildingType.ArcherFortress;
    public override float Cooldown => 2f;
    public override int Damage => 5;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override float Range => 3f;
    public override ResourceTransaction BuildCost => new ResourceTransaction(wood: 130, stone: 20);
    public override int NumProjectiles => 5;
    public override float ProjectileStartPostionRandomness => .3f;

    protected override void Attack()
    {
        base.Attack();
    }
}

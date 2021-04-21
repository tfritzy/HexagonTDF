using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkTower : AttackTower
{
    public override float Cooldown => AttackSpeed.VeryFast;
    public override int Damage => 2;
    public override float Range => RangeOptions.Short;
    public override BuildingType Type => BuildingType.SparkTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        {ResourceType.Wood, .6f},
        {ResourceType.Stone, .3f},
        {ResourceType.Food, .1f},
    };

    protected override void Attack()
    {
        Target.TakeDamage(Damage);
        GameObject projectile = Instantiate(
                Prefabs.Projectiles[Type],
                Target.GetComponent<Collider>().bounds.center,
                new Quaternion());
    }
}

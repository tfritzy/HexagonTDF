using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorTower : AttackTower
{
    public override float Cooldown => AttackSpeed.VerySlow;
    public override int Damage => 50;
    public override float Range => RangeOptions.VeryLong;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override BuildingType Type => BuildingType.MeteorTower;
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    protected override float ExplosionRadius => 3f;
    protected override int ExpectedNumberOfEnemiesHitByEachProjectile => 2;
    protected override float ProjectileSpeed => 20;
    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        { ResourceType.Stone, .9f},
        { ResourceType.Food, .1f},
    };

    protected override bool IsCollisionTarget(Character attacker, GameObject other)
    {
        // Only explodes on contact with ground to hit the most enemies possible.
        if (other.CompareTag(Constants.Tags.Hexagon))
        {
            return true;
        }

        return false;
    }
}

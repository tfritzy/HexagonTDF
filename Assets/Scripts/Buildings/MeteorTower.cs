// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MeteorTower : AttackTower
// {
//     public override float BaseCooldown => AttackSpeed.VerySlow;
//     public override int BaseDamage => 50;
//     public override float BaseRange => RangeOptions.VeryLong;
//     public override VerticalRegion AttackRegion => VerticalRegion.Ground;
//     public override BuildingType Type => BuildingType.MeteorTower;
//     public override Alliances Alliance => Alliances.Player;
//     public override Alliances Enemies => Alliances.Maltov;
//     protected override float ExplosionRadius => 3f;
//     protected override int ExpectedNumberOfEnemiesHitByEachProjectile => 2;
//     protected override float ProjectileSpeed => 20;

//     protected override bool IsCollisionTarget(Character attacker, GameObject other)
//     {
//         // Only explodes on contact with ground to hit the most enemies possible.
//         if (other.CompareTag(Constants.Tags.Hexagon))
//         {
//             return true;
//         }

//         return false;
//     }
// }

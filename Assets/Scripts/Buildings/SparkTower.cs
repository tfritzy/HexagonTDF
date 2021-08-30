// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class SparkTower : AttackTower
// {
//     public override float BaseCooldown => AttackSpeed.VeryVeryFast;
//     public override int BaseDamage => 1;
//     public override float BaseRange => RangeOptions.Short;
//     public override BuildingType Type => BuildingType.SparkTower;
//     public override Alliances Alliance => Alliances.Player;
//     public override Alliances Enemies => Alliances.Maltov;
//     public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
//     private const float DIST_BETWEEN_LIGHTNING_SEGMENTS = .25f;
//     private LineRenderer lr;

//     protected override void Setup()
//     {
//         base.Setup();
//         lr = this.GetComponent<LineRenderer>();
//         lr.enabled = false;
//     }

//     protected override void Attack()
//     {
//         Vector3 difference = Target.Position - projectileStartPosition.position;
//         int numPoints = (int)(difference.magnitude / DIST_BETWEEN_LIGHTNING_SEGMENTS);
//         lr.enabled = true;
//         difference = difference.normalized;
//         lr.positionCount = numPoints + 1;
//         for (int i = 0; i < numPoints; i++)
//         {
//             lr.SetPosition(
//                 i,
//                 projectileStartPosition.position +
//                 (difference * i * DIST_BETWEEN_LIGHTNING_SEGMENTS + Random.insideUnitSphere * DIST_BETWEEN_LIGHTNING_SEGMENTS));
//         }
//         lr.SetPosition(numPoints, Target.transform.position);

//         Target.TakeDamage(Damage, this);
//         GameObject projectile = Instantiate(
//                 Prefabs.Projectiles[Type],
//                 Target.GetComponent<Collider>().bounds.center,
//                 new Quaternion());
//     }

//     protected override void UpdateLoop()
//     {
//         base.UpdateLoop();

//         if (Time.time > lastAttackTime + .075f)
//         {
//             lr.enabled = false;
//         }
//     }
// }

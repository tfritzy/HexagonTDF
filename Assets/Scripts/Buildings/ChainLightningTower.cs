// using System.Collections.Generic;
// using UnityEngine;

// public class ChainLightningTower : AttackTower
// {
//     public override float BaseCooldown => AttackSpeed.Slow;
//     public override int BaseDamage => 10;
//     public override float BaseRange => RangeOptions.Medium;
//     public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
//     public override BuildingType Type => BuildingType.ChainLightningTower;
//     public override Alliances Alliance => Alliances.Player;
//     public override Alliances Enemies => Alliances.Maltov;
//     protected override int ExpectedNumberOfEnemiesHitByEachProjectile => 8;
//     private const float LIGHTNING_HOP_RANGE = 1f;
//     private const float LIGHTNING_FLASH_DURATION = .075f;
//     private List<GameObject> alreadyHitEnemies;
//     private LineRenderer lineRenderer;

//     protected override void Setup()
//     {
//         this.lineRenderer = this.GetComponent<LineRenderer>();
//         this.lineRenderer.enabled = false;
//         base.Setup();
//     }

//     protected override void Attack()
//     {
//         alreadyHitEnemies = new List<GameObject>();
//         Character current = Target;
//         List<Vector3> hitPositions = new List<Vector3>();
//         hitPositions.Add(this.projectileStartPosition.position);
//         while (current != null)
//         {
//             current.TakeDamage(this.Damage, this);
//             hitPositions.Add(current.Position);
//             alreadyHitEnemies.Add(current.gameObject);
//             current = findNextHop(current);
//         }

//         this.lineRenderer.enabled = true;
//         this.lineRenderer.positionCount = hitPositions.Count;
//         for (int i = 0; i < hitPositions.Count; i++)
//         {
//             this.lineRenderer.SetPosition(i, hitPositions[i]);
//         }
//     }

//     protected override void UpdateLoop()
//     {
//         if (Time.time > lastAttackTime + LIGHTNING_FLASH_DURATION)
//         {
//             this.lineRenderer.enabled = false;
//         }

//         base.UpdateLoop();
//     }

//     private Character findNextHop(Character current)
//     {
//         Collider[] hits = Physics.OverlapSphere(current.transform.position, LIGHTNING_HOP_RANGE, Constants.Layers.Characters);
//         foreach (Collider hit in hits)
//         {
//             if (alreadyHitEnemies.Contains(hit.gameObject) == false &&
//                 hit.TryGetComponent<Character>(out Character character))
//             {
//                 if (character.Alliance == this.Enemies)
//                 {
//                     return character;
//                 }
//             }
//         }

//         return null;
//     }


// }
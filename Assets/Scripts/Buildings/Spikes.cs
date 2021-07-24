using System.Collections.Generic;
using UnityEngine;

public class Spikes : AttackTower
{
    public override float BaseCooldown => AttackSpeed.Medium;
    public override int BaseDamage => 20;
    public override int BaseRange => RangeOptions.VeryVeryShort;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override BuildingType Type => BuildingType.Spikes;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override bool IsWalkable => true;
    protected override int ExpectedNumberOfEnemiesHitByEachProjectile => 2;

    private List<Damageable> damageablesInRange = new List<Damageable>();
    private float spikeMovementDistance;
    private GameObject spikesModel;
    private Vector3 initialSpikesPos;

    protected override void Setup()
    {
        base.Setup();
        spikesModel = transform.Find("Spikes").gameObject;
        initialSpikesPos = spikesModel.transform.position;
        spikeMovementDistance = spikesModel.GetComponent<MeshRenderer>().bounds.extents.y * 2;
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();
        retractSpikes();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (InterfaceUtility.TryGetInterface<Damageable>(out Damageable damageable, other.gameObject))
        {
            damageablesInRange.Add(damageable);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (InterfaceUtility.TryGetInterface<Damageable>(out Damageable damageable, other.gameObject))
        {
            damageablesInRange.Remove(damageable);
        }
    }

    protected override void Attack()
    {
        spikesModel.transform.position = initialSpikesPos + Vector3.up * spikeMovementDistance;

        foreach (Damageable damageable in damageablesInRange)
        {
            damageable.TakeDamage(this.Damage, this);
        }
    }

    protected override bool CanAttack()
    {
        TrimDamageableList();
        return damageablesInRange.Count > 0 && Time.time > lastAttackTime + Cooldown;
    }

    private void TrimDamageableList()
    {
        for (int i = damageablesInRange.Count - 1; i >= 0; i--)
        {
            if (damageablesInRange[i].IsNull())
            {
                damageablesInRange.RemoveAt(i);
            }
        }
    }

    private void retractSpikes()
    {
        if (spikesModel.transform.position.y > initialSpikesPos.y)
        {
            spikesModel.transform.position = spikesModel.transform.position + Vector3.down * (spikeMovementDistance / (this.Cooldown / 2)) * Time.deltaTime;
        }
    }
}
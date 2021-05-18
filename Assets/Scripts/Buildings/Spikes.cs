using System.Collections.Generic;
using UnityEngine;

public class Spikes : AttackTower
{
    public override float Cooldown => AttackSpeed.Medium;
    public override int Damage => 20;
    public override float Range => RangeOptions.VeryVeryShort;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override BuildingType Type => BuildingType.Spikes;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override bool IsWalkable => true;
    protected override int ExpectedNumberOfEnemiesHitByEachProjectile => 2;

    private List<Enemy> enemiesInRange = new List<Enemy>();
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
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInRange.Remove(enemy);
        }
    }

    protected override void Attack()
    {
        spikesModel.transform.position = initialSpikesPos + Vector3.up * spikeMovementDistance;

        foreach (Enemy enemy in enemiesInRange)
        {
            enemy.TakeDamage(this.Damage);
        }
    }

    protected override bool CanAttack()
    {
        TrimEnemyList();
        return enemiesInRange.Count > 0 && Time.time > lastAttackTime + Cooldown;
    }

    private void TrimEnemyList()
    {
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            if (enemiesInRange[i] == null)
            {
                enemiesInRange.RemoveAt(i);
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
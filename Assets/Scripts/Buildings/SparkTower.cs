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
    private const float DIST_BETWEEN_LIGHTNING_SEGMENTS = .5f;
    private LineRenderer lr;

    protected override void Setup()
    {
        base.Setup();
        lr = this.GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    protected override void Attack()
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 difference = Target.Position - projectileStartPosition;
        int numPoints = (int)(difference.magnitude / DIST_BETWEEN_LIGHTNING_SEGMENTS);
        lr.enabled = true;
        difference = difference.normalized;
        lr.positionCount = numPoints;
        for (int i = 0; i < numPoints; i++)
        {
            lr.SetPosition(i, projectileStartPosition + (difference * i * DIST_BETWEEN_LIGHTNING_SEGMENTS + Random.insideUnitSphere * DIST_BETWEEN_LIGHTNING_SEGMENTS));
        }

        Target.TakeDamage(Damage);
        GameObject projectile = Instantiate(
                Prefabs.Projectiles[Type],
                Target.GetComponent<Collider>().bounds.center,
                new Quaternion());
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();

        if (Time.time > lastAttackTime + .075f)
        {
            lr.enabled = false;
        }
    }

    private void SetLineRendererEnabledStatus()
    {

    }
}

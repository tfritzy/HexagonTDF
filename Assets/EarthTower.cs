using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EarthTower : AttackTower
{
    public override float Cooldown => .25f;
    public override int Damage => 2;
    public override float Range => 4;
    public float RockGenerationTime => 2f;
    public float MaxRocks = 3;
    public override BuildingType Type => BuildingType.EarthTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override ResourceTransaction BuildCost => new ResourceTransaction(wood: 25, gold: 100, stone: 250);
    private const float ROCK_ROTATION_RADIUS = .5f;
    private const float ROCK_ROTATION_TIME_SECONDS = 20f;

    protected override void Setup()
    {
        Projectiles = new List<GameObject>();
        this.centerRock = this.transform.Find("CenterRock");
        Orbit orbit = this.centerRock.GetComponent<Orbit>();
        orbit.Setup(new Vector3(0, .2f, 0), 0, .2f, null);
        base.Setup();
    }

    private List<GameObject> Projectiles;
    private float lastRockSpawnTime;
    private Transform centerRock;
    protected override void UpdateLoop()
    {
        base.UpdateLoop();

        if (Projectiles.Count == MaxRocks)
        {
            lastRockSpawnTime = Time.time;
        }

        if (Projectiles.Count < MaxRocks && Time.time > lastRockSpawnTime + RockGenerationTime)
        {
            CreateAndInitRock();
            lastRockSpawnTime = Time.time;
        }
    }

    protected override void Attack()
    {
        if (Projectiles.Count == 0)
        {
            return;
        }

        Projectile projectile = Projectiles.Last().GetComponent<Projectile>();
        projectile.enabled = true;
        projectile.gameObject.GetComponent<Orbit>().enabled = false;
        SetProjectileVelocity(projectile.gameObject);
        Projectiles.RemoveAt(Projectiles.Count - 1);
    }

    private void CreateAndInitRock()
    {
        float angle = this.Projectiles.Count * (360f / MaxRocks);
        GameObject rock = Instantiate(
            Prefabs.Projectiles[this.Type],
            projectileStartPosition + Vector3.right * ROCK_ROTATION_RADIUS,
            new Quaternion(),
            this.transform);
        rock.GetComponent<Projectile>().Initialize(this.DealDamageToEnemy, this);
        rock.GetComponent<Projectile>().enabled = false;
        Orbit orbit = rock.GetComponent<Orbit>();
        orbit.Setup(new Vector3(.5f, .5f, .5f), ROCK_ROTATION_TIME_SECONDS, .2f, centerRock, angle);
        Projectiles.Add(rock);
    }
}

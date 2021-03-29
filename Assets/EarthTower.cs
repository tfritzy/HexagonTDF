using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthTower : AttackTower
{
    public override float Cooldown => 2;
    public override int Damage => 1;
    public override float Range => 4;
    public float MaxRocks = 3;
    public override BuildingType Type => BuildingType.EarthTower;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
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

        if (Projectiles.Count < MaxRocks && Time.time > lastRockSpawnTime + Cooldown)
        {
            CreateAndInitRock();
            lastRockSpawnTime = Time.time;
        }
    }

    protected override void Attack()
    {

    }

    private void CreateAndInitRock()
    {
        GameObject rock = Instantiate(
            Prefabs.Projectiles[this.Type],
            projectileStartPosition + Vector3.right * ROCK_ROTATION_RADIUS,
            new Quaternion(),
            this.transform);
        Orbit orbit = rock.GetComponent<Orbit>();
        orbit.Setup(new Vector3(.5f, .5f, .5f), ROCK_ROTATION_TIME_SECONDS, .2f, centerRock);
        Projectiles.Add(rock);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    public override BuildingType Type => BuildingType.Barracks;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public override int StartingHealth => 100;
    public override float Power => int.MaxValue;
    public int BarracksIndex;
    public override float BaseCooldown => float.MaxValue / 2;
    public override int BaseDamage => throw new System.NotImplementedException();
    public override int BaseRange => throw new System.NotImplementedException();
    public override VerticalRegion AttackRegion => throw new System.NotImplementedException();

    private Vector2Int exitGridPosition;
    private const float SPAWN_INTERVAL_DURATION_S = 15;
    private readonly List<int> tricklePowerCurve = new List<int>()
    {
        1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,
        39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,
        75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100
    };
    private readonly List<int> burstSpawnPowerCurve = new List<int>()
    {
        0,6,0,12,0,18,0,24,0,30,0,36,0,42,0,48,0,54,0,60,0,66,0,72,0,78,0,84,0,90,0,96,0,102,0,108,0,114,0,120,
        0,126,0,132,0,138,0,144,0,150,0,156,0,162,0,168,0,174,0,180,0,186,0,192,0,198,0,204,0,210,0,216,0,222,0,
        228,0,234,0,240,0,246,0,252,0,258,0,264,0,270,0,276,0,282,0,288,0,294,0,300
    };

    private int currentSpawnInterval;
    private float levelStartTime;
    List<EnemyType> spawnableEnemies = new List<EnemyType>()
    {
        // EnemyType.StickGuy,
        // EnemyType.Spellcaster,
        // EnemyType.ShieldKnight,
        EnemyType.Velociraptor,
    };
    protected override void Setup()
    {
        base.Setup();
        this.levelStartTime = Time.time;
        this.exitGridPosition = Managers.Board.GetNextStepInPathToSource(
            Managers.Board.Trebuchet.GridPosition,
            this.GridPosition).Position;
        this.BarracksIndex = Managers.Board.Barracks.IndexOf(this);
    }
    private List<SpawnDetail> spawnTimings;

    private class SpawnDetail
    {
        public float Time;
        public EnemyType Type;
    }

    private float lastSpawnTime = 0f;
    private int currentSpawnIndex;
    protected override void UpdateLoop()
    {
        base.UpdateLoop();

        if (currentSpawnInterval >= tricklePowerCurve.Count)
        {
            return;
        }

        if (Time.time >= levelStartTime + currentSpawnInterval * SPAWN_INTERVAL_DURATION_S)
        {
            float powerBudget = tricklePowerCurve[currentSpawnInterval];
            System.Random random = new System.Random(currentSpawnInterval);
            int roll = random.Next(0, Managers.Board.Barracks.Count);
            if (roll == this.BarracksIndex)
            {
                powerBudget += burstSpawnPowerCurve[currentSpawnInterval];
            }
            List<EnemyType> spawnFormation = RollSpawnFormation(powerBudget);
            CalculateSpawnTimings(powerBudget, spawnFormation);
            currentSpawnIndex = 0;
            currentSpawnInterval += 1;
        }

        if (currentSpawnIndex < spawnTimings.Count && Time.time > spawnTimings[currentSpawnIndex].Time)
        {
            GameObject enemy = Instantiate(
                Prefabs.Enemies[spawnTimings[currentSpawnIndex].Type].gameObject,
                this.transform.position,
                new Quaternion(),
                null);
            Enemy enemyMono = enemy.GetComponent<Enemy>();
            enemyMono.AddRigidbody();
            enemyMono.SetInitialPosition(this.GridPosition);
            lastSpawnTime = Time.time;
            currentSpawnIndex += 1;
        }
    }

    private List<EnemyType> RollSpawnFormation(float powerBudget)
    {
        List<EnemyType> spawnFormation = new List<EnemyType>();
        int numUnitTypes = 1;
        if (Random.Range(0, 2) == 1)
        {
            numUnitTypes = 2;
        }

        int highestAffordableEnemyIndex = 0;
        while (highestAffordableEnemyIndex < spawnableEnemies.Count &&
               Prefabs.Enemies[spawnableEnemies[highestAffordableEnemyIndex]].BasePower < powerBudget)
        {
            highestAffordableEnemyIndex += 1;
        }
        highestAffordableEnemyIndex -= 1;

        List<EnemyType> affordableEnemies = new List<EnemyType>();
        for (int i = 0; i <= highestAffordableEnemyIndex; i++)
        {
            affordableEnemies.Add(spawnableEnemies[i]);
        }

        for (int i = 0; i < numUnitTypes; i++)
        {
            if (affordableEnemies.Count == 0)
            {
                continue;
            }

            EnemyType chosenEnemyType = affordableEnemies[Random.Range(0, affordableEnemies.Count)];
            spawnFormation.Add(chosenEnemyType);
            affordableEnemies.Remove(chosenEnemyType);
        }

        return spawnFormation;
    }

    private void CalculateSpawnTimings(float powerBudget, List<EnemyType> spawnFormation)
    {
        spawnTimings = new List<SpawnDetail>();
        int spawnIndex = 0;

        if (spawnFormation.Count == 0)
        {
            return;
        }

        while (powerBudget >= Prefabs.Enemies[spawnFormation[spawnIndex % spawnFormation.Count]].BasePower)
        {
            EnemyType type = spawnFormation[spawnIndex % spawnFormation.Count];
            powerBudget -= Prefabs.Enemies[type].BasePower;
            SpawnDetail detail = new SpawnDetail() { Type = type };
            spawnTimings.Add(detail);
        }

        float timeBetweenEnemies = SPAWN_INTERVAL_DURATION_S / (spawnTimings.Count);
        float offset = Random.Range(0, timeBetweenEnemies * .95f);
        for (int i = 0; i < spawnTimings.Count; i++)
        {
            spawnTimings[i].Time = Time.time + offset + timeBetweenEnemies * i;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    public override BuildingType Type => BuildingType.Barracks;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public override int StartingHealth => 900;
    public override float Power => int.MaxValue;
    public int BarracksIndex;
    public override float BaseCooldown => float.MaxValue / 2;
    public override int BaseDamage => throw new System.NotImplementedException();
    public override float BaseRange => throw new System.NotImplementedException();
    public override VerticalRegion AttackRegion => throw new System.NotImplementedException();

    private const float goldTaperPercentPerSecond = 0.00333333333333333f;
    private Vector2Int exitGridPosition;
    private readonly List<int> tricklePowerCurve = new List<int>()
    {
        0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,
        39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,
        75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100
    };
    private readonly List<int> burstSpawnPowerCurve = new List<int>()
    {
        0, 2, 6, 10, 15, 20, 25, 31, 37, 43, 49, 56, 62, 69, 76, 83, 91, 98, 105, 113, 121, 129, 137,
        145, 153, 162, 170, 179, 187, 196, 205, 214, 223, 232, 241, 250, 260, 269, 279, 288, 298, 308,
        317, 327, 337, 347, 357, 367, 378, 388, 398, 408, 419, 429, 440, 451, 461, 472, 483, 494, 505,
        516, 527, 538, 549, 560, 571, 582, 594, 605, 617, 628, 640, 651, 663, 674, 686, 698, 710, 722,
        733, 745, 757, 769, 781, 794, 806, 818, 830, 842, 855, 867, 880, 892, 904, 917, 930, 942, 955,
        968, 980
    };

    private int currentWave;
    private float levelStartTime;
    List<EnemyType> spawnableEnemies = new List<EnemyType>()
    {
        EnemyType.StickGuy,
        EnemyType.Spellcaster,
        EnemyType.ShieldKnight,
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

        if (currentWave >= tricklePowerCurve.Count)
        {
            return;
        }

        if (Time.time >= levelStartTime + currentWave * Constants.BALANCE_INTERVAL_SECONDS)
        {
            float powerBudget = tricklePowerCurve[currentWave];
            System.Random random = new System.Random(currentWave);
            int roll = random.Next(0, Managers.Board.Barracks.Count);
            if (roll == this.BarracksIndex)
            {
                powerBudget += burstSpawnPowerCurve[currentWave];
            }
            Managers.DebugDetails.InformOfWaveStart(currentWave, powerBudget);
            List<EnemyType> spawnFormation = RollSpawnFormation(powerBudget);
            CalculateSpawnTimings(powerBudget, spawnFormation);
            currentSpawnIndex = 0;
            currentWave += 1;
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
            enemyMono.GoldTaperAmount = 1 - Mathf.Min((Time.time - levelStartTime) * goldTaperPercentPerSecond, .9f);
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

        float timeBetweenEnemies = Constants.BALANCE_INTERVAL_SECONDS / (spawnTimings.Count);
        float offset = Random.Range(0, timeBetweenEnemies * .95f);
        for (int i = 0; i < spawnTimings.Count; i++)
        {
            spawnTimings[i].Time = Time.time + offset + timeBetweenEnemies * i;
        }
    }

    protected override void Die()
    {
        Managers.Board.Barracks.Remove(this);
        base.Die();
    }
}

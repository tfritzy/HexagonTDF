using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    public override BuildingType Type => BuildingType.Barracks;
    public override Alliances Alliance => Alliances.Maltov;
    public override Alliances Enemies => Alliances.Player;
    public override int StartingHealth => (int)(900 * GameState.LevelPowerMultiplier);
    public override float Power => int.MaxValue;
    public int BarracksIndex;
    public override float BaseCooldown => float.MaxValue / 2;
    public override int BaseDamage => throw new System.NotImplementedException();
    public override float BaseRange => throw new System.NotImplementedException();
    public override VerticalRegion AttackRegion => throw new System.NotImplementedException();

    private const float goldTaperPercentPerSecond = 0.00333333333333333f;
    private Vector2Int exitGridPosition;

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

        if (Time.time >= levelStartTime + currentWave * Constants.BALANCE_INTERVAL_SECONDS)
        {
            float powerBudget = currentWave;
            System.Random random = new System.Random(currentWave);

            if (currentWave % 2 == 1)
            {
                this.BarracksIndex = Managers.Board.Barracks.IndexOf(this);
                int roll = random.Next(0, Managers.Board.Barracks.Count);
                if (roll == this.BarracksIndex)
                {
                    powerBudget += Mathf.Pow(currentWave * 2, 1.3f);
                }
            }

            Managers.DebugDetails.InformOfWaveStart(currentWave, powerBudget);
            List<EnemyType> spawnFormation = RollSpawnFormation(powerBudget);
            CalculateSpawnTimings(powerBudget, spawnFormation);
            currentSpawnIndex = 0;
            currentWave += 1;
        }

        if (currentSpawnIndex < spawnTimings.Count && Time.time > spawnTimings[currentSpawnIndex].Time)
        {
            Vector3 enemyOffset = Random.insideUnitSphere * Constants.HEXAGON_r;
            enemyOffset.y = 0;
            GameObject enemy = Instantiate(
                Prefabs.Enemies[spawnTimings[currentSpawnIndex].Type].gameObject,
                this.transform.position + enemyOffset,
                new Quaternion(),
                null);
            Enemy enemyMono = enemy.GetComponent<Enemy>();
            enemyMono.PositionOffset = enemyOffset;
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

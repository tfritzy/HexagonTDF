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

    public override float Cooldown => float.MaxValue / 2;
    public override int Damage => throw new System.NotImplementedException();
    public override int Range => throw new System.NotImplementedException();
    public override VerticalRegion AttackRegion => throw new System.NotImplementedException();

    public EnemyType[] Soldiers;
    private Vector2Int exitGridPosition;
    private const float SPAWN_INTERVAL_DURATION_S = 15;
    private readonly List<int> tricklePowerCurve = new List<int>()
    {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19
    };
    private readonly List<int> burstSpawnPowerCurve = new List<int>()
    {
        0, 0, 6, 0, 12, 0, 18, 0, 24, 0, 30, 0, 36, 0, 42, 0, 48, 0, 54, 0
    };
    private int currentSpawnInterval;
    private float levelStartTime;
    List<EnemyType> spawnableEnemies = new List<EnemyType>()
    {
        EnemyType.StickGuy,
        EnemyType.Spellcaster
    };
    private List<EnemyType> spawnFormation;

    protected override void Setup()
    {
        base.Setup();
        this.levelStartTime = Time.time;
        this.exitGridPosition = Managers.Board.GetNextStepInPathToSource(Managers.Board.Trebuchet.GridPosition, this.GridPosition).Position;
    }

    private float lastSpawnTime = 0f;
    protected override void UpdateLoop()
    {
        base.UpdateLoop();

        if (Time.time > levelStartTime + currentSpawnInterval * SPAWN_INTERVAL_DURATION_S)
        {
            float powerBudget = tricklePowerCurve[currentSpawnInterval];
            currentSpawnInterval += 1;
        }

        if (Time.time > lastSpawnTime + 5f && NumSoldiers > 0)
        {
            GameObject enemy = Instantiate(Prefabs.Enemies[Soldiers[Random.Range(0, Soldiers.Length)]].gameObject, this.transform.position, new Quaternion(), null);
            Enemy enemyMono = enemy.GetComponent<Enemy>();
            enemyMono.SetPower(1f, 1f);
            enemyMono.AddRigidbody();
            enemyMono.SetInitialPosition(this.GridPosition);
            lastSpawnTime = Time.time;
            NumSoldiers -= 1;
        }
    }

    private void RollSpawnFormation(int powerBudget)
    {
        spawnFormation = new List<EnemyType>();
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
    }
}

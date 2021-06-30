using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    public int NumSoldiers;
    public override BuildingType Type => BuildingType.Barracks;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public override int StartingHealth => 100;
    public override float Power => int.MaxValue;
    public EnemyType[] Soldiers;
    private Vector2Int exitGridPosition;

    protected override void Setup()
    {
        base.Setup();

        NumSoldiers = Random.Range(20, 30);
        exitGridPosition = Managers.Board.GetNextStepInPathToSource(Managers.Board.Orbs[0].GridPosition, this.GridPosition).Position;
    }

    private float lastSpawnTime = 0f;
    protected override void UpdateLoop()
    {
        base.UpdateLoop();

        if (Time.time > lastSpawnTime + 5f && NumSoldiers > 0 && Helpers.IsGridPosOccupiedByCharacter(exitGridPosition) == false)
        {
            GameObject enemy = Instantiate(Prefabs.Enemies[Soldiers[Random.Range(0, Soldiers.Length)]].gameObject, this.transform.position, new Quaternion(), null);
            Enemy enemyMono = enemy.GetComponent<Enemy>();
            enemyMono.SetPower(1f, 1f);
            enemyMono.AddRigidbody();
            enemyMono.SetPathingPositions(this.GridPosition, exitGridPosition, false);
            lastSpawnTime = Time.time;
            NumSoldiers -= 1;
        }
    }
}

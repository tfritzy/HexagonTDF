using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Portal : Building
{
    public override BuildingType Type => BuildingType.Portal;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public GameObject Dot;
    public float SavedPower;
    public List<Vector2Int> PathToSource;
    public Guid PathId;
    public override Dictionary<ResourceType, float> CostRatio => costRatio;
    public override float Power => 100;
    private Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float>
    {
        {ResourceType.Stone, 1f},
    };
    private float levelStartTime;
    private const float MaxSpawnSpeed = .25f;
    private readonly List<float> SavedPowerGainRate30SecondInterval = new List<float>()
    {
        .2f,
        .2f,
        .7f,
        1.6f,
        3.3f,
        8.2f,
        15.5f,
        24.4f,
        33.3f,
        42.2f,
        51.1f
    };

    private readonly List<EnemyType> enemies = new List<EnemyType>()
    {
        EnemyType.Wisp,
        EnemyType.Tetriquiter,
        EnemyType.Sqorpin
    };

    protected override void Setup()
    {
        dots = new List<GameObject>();
        RecalculatePath();
        base.Setup();
        levelStartTime = Time.time;
    }

    private List<GameObject> dots;
    public void RecalculatePath()
    {
        List<Vector2Int> oldPath = PathToSource;
        PathToSource = Helpers.FindPath(Managers.Map.Hexagons, Position, Managers.Map.Source.Position);

        if (PathToSource == null)
        {
            throw new System.NullReferenceException($"Portal was unable to find path to source.");
        }

        bool arePathsSame = true;
        if (oldPath != null && oldPath.Count == PathToSource.Count)
        {
            for (int i = 0; i < PathToSource.Count; i++)
            {
                if (PathToSource[i] != oldPath[i])
                {
                    arePathsSame = false;
                }
            }
        }
        else
        {
            arePathsSame = false;
        }

        if (arePathsSame == false)
        {
            PathId = Guid.NewGuid();
            foreach (GameObject dot in dots)
            {
                Destroy(dot);
            }

            foreach (Vector2Int position in PathToSource)
            {
                dots.Add(Instantiate(Dot, Hexagon.ToWorldPosition(position) + Vector3.up, new Quaternion(), null));
            }
        }
    }

    private float lastSpawnTime;
    private float lastSavedPowerTime;
    protected override void UpdateLoop()
    {
        EnemyType? enemyToSpawn = GetHighestPurchasableEnemy();

        if (enemyToSpawn.HasValue && Time.time > lastSpawnTime + MaxSpawnSpeed)
        {
            GameObject enemy = Instantiate(Prefabs.Enemies[enemyToSpawn.Value].gameObject, this.transform.position + Vector3.up, new Quaternion(), null);
            Enemy enemyMono = enemy.GetComponent<Enemy>();
            enemyMono.SetPortal(this);
            SavedPower -= enemyMono.Power;
            lastSpawnTime = Time.time;
        }

        if (Time.time > lastSavedPowerTime + 1f)
        {
            SavedPower += SavedPowerGainRate30SecondInterval[(int)((Time.time - levelStartTime) / 30)];
            lastSavedPowerTime = Time.time;
        }
    }

    private EnemyType? GetHighestPurchasableEnemy()
    {
        int i = enemies.Count - 1;
        while (i >= 0 && Prefabs.Enemies[enemies[i]].Power > SavedPower)
        {
            i -= 1;
        }

        if (i == -1)
        {
            return null;
        }

        return enemies[i];
    }
}

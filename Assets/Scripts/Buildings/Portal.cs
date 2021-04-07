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
    private LineRenderer lineRenderer;
    private readonly List<float> SavedPowerGainRate30SecondInterval = new List<float>()
    {
        0.2f,
        0.26f,
        0.838f,
        1.9894f,
        4.28622f,
        10.472086f,
        20.9137118f,
        36.08782534f,
        55.814172942f,
        81.4584248246f,
        114.79595227198f,
    };

    private readonly List<EnemyType> enemies = new List<EnemyType>()
    {
        EnemyType.Tetriquiter,
        EnemyType.Sqorpin,
        EnemyType.Dode,
        EnemyType.Icid,
        EnemyType.Octahedor,
    };

    protected override void Setup()
    {
        lineRenderer = transform.Find("Path").GetComponent<LineRenderer>();
        RecalculatePath();
        base.Setup();
        levelStartTime = Time.time;
    }

    public void RecalculatePath()
    {
        List<Vector2Int> oldPath = PathToSource;
        PathToSource = Helpers.FindPath(Managers.Map.Hexagons, Managers.Map.GetBuildingTypeMap(), Position, Managers.Map.Source.Position);

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

            lineRenderer.positionCount = PathToSource.Count + 1;
            lineRenderer.SetPosition(0, this.transform.position);
            for (int i = 1; i < lineRenderer.positionCount; i++)
            {
                Vector2Int pos = PathToSource[i - 1];
                lineRenderer.SetPosition(i, Managers.Map.Hexagons[pos.x, pos.y].transform.position + Vector3.up * .01f);
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

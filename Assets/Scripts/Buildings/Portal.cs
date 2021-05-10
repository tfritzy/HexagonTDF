using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Portal : Building
{
    public override BuildingType Type => BuildingType.Portal;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public List<Vector2Int> PathToSource;
    public Guid PathId;
    public override float Power => 100;
    private int _currentWave;
    public int CurrentWave
    {
        get { return _currentWave; }
        set
        {
            _currentWave = value;
            startWaveCounter.text = _currentWave.ToString();
        }
    }
    private const float WAVE_DURATION_SEC = 30f;
    private const float DEFAULT_SEC_BETWEEN_SPAWN = 1f;
    private float levelStartTime;
    private float lastWaveStartTime;
    private const float MaxSpawnSpeed = .25f;
    private LineRenderer lineRenderer;
    private List<WaveType> waveTypes = Enum.GetValues(typeof(WaveType)).Cast<WaveType>().ToList();
    private readonly List<float> PowerPerWave = new List<float>()
    {
        0f,
        0.26f,
        0.838f,
        2.0894f,
        4.81622f,
        8.961086f,
        14.3494118f,
        21.35423534f,
        30.460505942f,
        42.2986577246f,
        57.68825504198f,
    };
    private List<GameObject> pathCorners;
    private float WaveTypeHealthMultiplier;
    private float WaveTypeSpawnSpeedMultiplier;
    private bool HasPeriodicLargeMinions;
    private float PeriodicLargeHealthModifier;
    private EnemyType currentWaveEnemy;
    private GameObject startWaveDialog;
    private Text startWaveTimer;
    private Text startWaveCounter;
    private Dictionary<ResourceType, Text> startWaveResourceBonusTexts;
    private readonly List<EnemyType> enemies = new List<EnemyType>()
    {
        EnemyType.StickGuy,
    };
    private readonly List<ResourceType> bonusResourceTypes = new List<ResourceType>()
    {
        ResourceType.Gold
    };

    protected override void Setup()
    {
        lineRenderer = transform.Find("Path").GetComponent<LineRenderer>();
        startWaveDialog = Managers.Canvas.Find("StartWaveDialog").gameObject;
        startWaveTimer = startWaveDialog.transform.Find("Content Group").Find("Time").GetComponent<Text>();
        startWaveCounter = startWaveDialog.transform.Find("Avatar Frame").Find("Mask").Find("WaveCounter").GetComponent<Text>();
        startWaveResourceBonusTexts = new Dictionary<ResourceType, Text>();
        Transform resources = startWaveDialog.transform.Find("Content Group").Find("Resources");
        startWaveResourceBonusTexts[ResourceType.Gold] = resources.Find("Gold").Find("Text").GetComponent<Text>();
        startWaveDialog.SetActive(false);
        this.pathCorners = new List<GameObject>();
        RecalculatePath();
        base.Setup();
        levelStartTime = Time.time;
        lastWaveStartTime = Time.time + WAVE_DURATION_SEC * 2;
        CurrentWave = 1;
        RollWaveType();
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

            ResetLineRenderer();
        }
    }

    public void StartWaveEarly()
    {
        foreach (ResourceType type in bonusResourceTypes)
        {
            Managers.ResourceStore.Add(type, GetStartEarlyBonus(type));
        }
        lastWaveStartTime = Time.time;
        startWaveDialog.SetActive(false);
    }

    private void ResetLineRenderer()
    {
        lineRenderer.positionCount = PathToSource.Count + 1;
        lineRenderer.SetPosition(0, this.transform.position);
        int i = 0;
        for (i = 1; i < lineRenderer.positionCount; i++)
        {
            Vector2Int pos = PathToSource[i - 1];
            lineRenderer.SetPosition(i, Managers.Map.Hexagons[pos.x, pos.y].transform.position + Vector3.up * .01f);
        }

        for (i = 0; i < PathToSource.Count; i++)
        {
            if (i >= pathCorners.Count)
            {
                pathCorners.Add(Instantiate(Prefabs.PathCorner));
            }

            Vector2Int pos = PathToSource[i];
            pathCorners[i].SetActive(true);
            pathCorners[i].transform.position = Managers.Map.Hexagons[pos.x, pos.y].transform.position + Vector3.up * .01f;
        }

        for (; i < pathCorners.Count; i++)
        {
            pathCorners[i].SetActive(false);
        }
    }

    private float lastSpawnTime;
    protected override void UpdateLoop()
    {
        if (Time.time - lastWaveStartTime < 0)
        {
            UpdateStartWaveDialog();
            startWaveTimer.text = $"{(int)(lastWaveStartTime - Time.time)}s";
            startWaveDialog.SetActive(true);
            return;
        }

        if (Time.time - lastWaveStartTime >= WAVE_DURATION_SEC)
        {
            CurrentWave += 1;
            RollWaveType();
            lastWaveStartTime = Time.time + WAVE_DURATION_SEC;
            return;
        }

        startWaveDialog.SetActive(false);
        if (Time.time > lastSpawnTime + DEFAULT_SEC_BETWEEN_SPAWN * WaveTypeSpawnSpeedMultiplier)
        {
            GameObject enemy = Instantiate(Prefabs.Enemies[currentWaveEnemy].gameObject, this.transform.position, new Quaternion(), null);
            Enemy enemyMono = enemy.GetComponent<Enemy>();
            enemyMono.SetPower(PowerPerWave[CurrentWave], WaveTypeHealthMultiplier);
            enemyMono.SetPortal(this);
            lastSpawnTime = Time.time;
        }
    }

    private void RollWaveType()
    {
        currentWaveEnemy = enemies[UnityEngine.Random.Range(0, enemies.Count)];
        int availableWaves = CurrentWave > 3 ? waveTypes.Count : waveTypes.Count - 2;
        WaveType waveType = waveTypes[UnityEngine.Random.Range(0, availableWaves)];
        HasPeriodicLargeMinions = false;
        switch (waveType)
        {
            case (WaveType.Clustered):
                WaveTypeHealthMultiplier = .2f;
                WaveTypeSpawnSpeedMultiplier = .2f;
                break;
            case (WaveType.ClusteredWithBiggies):
                HasPeriodicLargeMinions = true;
                PeriodicLargeHealthModifier = .6f;
                WaveTypeSpawnSpeedMultiplier = .5f;
                WaveTypeHealthMultiplier = .2f;
                break;
            case (WaveType.Normal):
                WaveTypeSpawnSpeedMultiplier = 1f;
                WaveTypeHealthMultiplier = 1f;
                break;
            case (WaveType.Spread):
                WaveTypeSpawnSpeedMultiplier = 3f;
                WaveTypeHealthMultiplier = 3f;
                break;
        }
    }

    private void UpdateStartWaveDialog()
    {
        foreach (ResourceType type in bonusResourceTypes)
        {
            startWaveResourceBonusTexts[type].text = GetStartEarlyBonus(type).ToString();
        }
    }

    private int GetStartEarlyBonus(ResourceType type)
    {
        return (int)(((lastWaveStartTime - Time.time) * 1.25f * PowerPerWave[CurrentWave]));
    }
}

﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    OverworldTerrainGenerator generator;
    public GameObject Tile;
    public GameObject City;
    public const int Seed = 2;
    public List<OverworldFortress> Fortresses;

    private Pool pool;
    public const int NUM_SEGMENTS_SPAWNED_HEIGHT = 30;
    public const int NUM_SEGMENTS_SPAWNED_WIDTH = 6;
    private float tileWidth;
    private Camera cam;
    private Vector2Int spawnedLowPos;
    private Vector2Int targetLowPos;
    private Dictionary<Vector2Int, OverworldSegment> mapCache;
    private bool initialized;
    private Dictionary<Vector2Int, GameObject> activeTiles;
    private System.Random Random = new System.Random(Seed);

    private enum ObjectType
    {
        Tile = 0,
        City = 1,
    }

    // Start is called before the first frame update
    async void Start()
    {
        generator = this.transform.GetComponent<OverworldTerrainGenerator>();
        tileWidth = Tile.GetComponent<MeshRenderer>().bounds.extents.x * 2;
        cam = Camera.main;
        spawnedLowPos.y = 0;
        mapCache = new Dictionary<Vector2Int, OverworldSegment>();
        activeTiles = new Dictionary<Vector2Int, GameObject>();
        Fortresses = new List<OverworldFortress>();

        pool = new Pool(
            new Dictionary<int, GameObject>
            {
                {
                    (int)ObjectType.Tile, Tile
                },
                {
                    (int)ObjectType.City, City
                }
            }
        );

        for (int y = 0; y <= NUM_SEGMENTS_SPAWNED_HEIGHT; y++)
        {
            int xMax = y == NUM_SEGMENTS_SPAWNED_HEIGHT ? 1 : NUM_SEGMENTS_SPAWNED_WIDTH;
            for (int x = 0; x < xMax; x++)
            {
                GameObject tile = pool.GetObject((int)ObjectType.Tile);
                await formatSegment(new Vector2Int(x, y), tile);
            }
        }
        initialized = true;
    }

    async void Update()
    {
        if (!initialized)
        {
            return;
        }

        if (cam.transform.position.z > ((targetLowPos.y + NUM_SEGMENTS_SPAWNED_HEIGHT * .33f) * tileWidth))
        {
            targetLowPos.y += 1;
        }

        if (cam.transform.position.z < ((targetLowPos.y + NUM_SEGMENTS_SPAWNED_HEIGHT * .33f) * tileWidth))
        {
            targetLowPos.y -= 1;
        }

        await SpawnTiles();
    }

    private async Task SpawnTiles()
    {
        if (spawnedLowPos.y < targetLowPos.y)
        {
            this.ReturnToPool(activeTiles[spawnedLowPos]);
            spawnedLowPos.x += 1;
            if (spawnedLowPos.x >= NUM_SEGMENTS_SPAWNED_WIDTH)
            {
                spawnedLowPos.y += 1;
                spawnedLowPos.x = 0;
            }

            GameObject tile = this.pool.GetObject((int)ObjectType.Tile);
            tile.transform.SetParent(this.transform);
            await formatSegment(new Vector2Int(spawnedLowPos.x, spawnedLowPos.y + NUM_SEGMENTS_SPAWNED_HEIGHT), tile);
        }
        else if (spawnedLowPos.y > targetLowPos.y)
        {
            this.ReturnToPool(activeTiles[spawnedLowPos + Vector2Int.up * NUM_SEGMENTS_SPAWNED_HEIGHT]);
            spawnedLowPos.x -= 1;
            if (spawnedLowPos.x < 0)
            {
                spawnedLowPos.x = NUM_SEGMENTS_SPAWNED_WIDTH - 1;
                spawnedLowPos.y -= 1;
            }

            GameObject tile = this.pool.GetObject((int)ObjectType.Tile);
            await formatSegment(new Vector2Int(spawnedLowPos.x, spawnedLowPos.y), tile);
        }
    }

    private async Task formatSegment(Vector2Int pos, GameObject tile)
    {
        tile.name = $"Map segment {pos.x},{pos.y}";
        tile.transform.position = new Vector3(tileWidth * pos.x, 0, tileWidth * pos.y);
        Vector2Int gridPos = new Vector2Int(pos.x, pos.y);
        this.activeTiles[gridPos] = tile;
        if (mapCache.ContainsKey(gridPos) == false)
        {
            OverworldSegment segment;
            if (pos.x == NUM_SEGMENTS_SPAWNED_WIDTH - 1)
            {
                segment = await generator.GetSegment(pos.x, pos.y, Seed, -.5f, 1);
            }
            else if (pos.x == 0)
            {
                segment = await generator.GetSegment(pos.x, pos.y, Seed, .5f, .5f);
            }
            else
            {
                segment = await generator.GetSegment(pos.x, pos.y, Seed);
            }

            segment.Texture = generator.GetTextureOfMap(segment.Points);

            mapCache[gridPos] = segment;
        }

        if (gridPos.y > 0 && mapCache[gridPos].HasCity)
        {
            SpawnFortress(tile, pos);
        }

        tile.GetComponent<MeshRenderer>().material.mainTexture = mapCache[gridPos].Texture;
    }

    const float expectedCitiesPerRow = .5f;
    const float powerGainedPerCity = .1f;
    private void SpawnFortress(GameObject tile, Vector2Int pos)
    {
        float variance = 1 + (float)Random.NextDouble() / 2 - .25f;
        float powerMultiplier = Mathf.Pow(1 + powerGainedPerCity * expectedCitiesPerRow, pos.y) * variance;

        GameObject fortress = pool.GetObject((int)ObjectType.City);
        fortress.transform.position = tile.transform.position;
        fortress.transform.parent = tile.transform;
        OverworldFortress fortressScript = fortress.GetComponent<OverworldFortress>();
        Fortresses.Add(fortressScript);
        fortressScript.Setup(powerMultiplier, pos);
        int mapWidth = OverworldManager.NUM_SEGMENTS_SPAWNED_WIDTH;
        Fortresses.Sort((f1, f2) => (f2.Position.y * mapWidth + f2.Position.x) - (f1.Position.y * mapWidth + f1.Position.x));
        fortressScript.CreateLinks();
    }

    private void ReturnToPool(GameObject tile)
    {
        OverworldFortress fortress = tile.GetComponentInChildren<OverworldFortress>();
        if (fortress != null)
        {
            Fortresses.Remove(fortress);
        }

        foreach (PoolObject poolObject in tile.GetComponentsInChildren<PoolObject>())
        {
            poolObject.ReturnToPool();
        }
    }

    public OverworldSegment GetSegment(Vector2Int pos)
    {
        if (mapCache.ContainsKey(pos))
        {
            return mapCache[pos];
        }
        else
        {
            throw new System.Exception("The requested tile hasn't been generated before.");
        }
    }
}

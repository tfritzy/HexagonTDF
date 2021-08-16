using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    IslandGenerator generator;
    public GameObject Tile;
    public int Seed;

    private const int NUM_SEGMENTS_SPAWNED_HEIGHT = 50;
    private const int NUM_SEGMENTS_SPAWNED_WIDTH = 6;
    private float tileWidth;
    private Camera cam;
    private LinkedList<GameObject> mapTiles;
    private int spawnedMapLowY;
    private int spawnedMapLowX;
    private int targetMapLowY;
    private int targetMapLowX;
    private Dictionary<Vector2Int, MapSegment> mapCache;
    private bool initialized;

    private struct MapSegment
    {
        public Texture2D Texture;
        public IslandGenerator.MapPoint[,] Map;
    }

    // Start is called before the first frame update
    async void Start()
    {
        generator = this.transform.GetComponent<IslandGenerator>();
        tileWidth = Tile.GetComponent<MeshRenderer>().bounds.extents.x * 2;
        cam = Camera.main;
        spawnedMapLowY = 0;
        mapTiles = new LinkedList<GameObject>();
        mapCache = new Dictionary<Vector2Int, MapSegment>();

        for (int y = 0; y <= NUM_SEGMENTS_SPAWNED_HEIGHT; y++)
        {
            for (int x = 0; x < NUM_SEGMENTS_SPAWNED_WIDTH; x++)
            {
                GameObject tile = this.createSegment();
                mapTiles.AddLast(tile);
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

        if (cam.transform.position.z > ((targetMapLowY + NUM_SEGMENTS_SPAWNED_HEIGHT * .33f) * tileWidth))
        {
            targetMapLowX += 1;
            if (targetMapLowX >= NUM_SEGMENTS_SPAWNED_WIDTH)
            {
                targetMapLowX = 0;
                targetMapLowY += 1;
            }
        }

        if (cam.transform.position.z < ((targetMapLowY + NUM_SEGMENTS_SPAWNED_HEIGHT * .33f) * tileWidth))
        {
            targetMapLowX -= 1;
            if (targetMapLowX < 0)
            {
                targetMapLowX = NUM_SEGMENTS_SPAWNED_WIDTH - 1;
                targetMapLowY -= 1;
            }
        }

        await SpawnTiles();
    }

    private readonly object tilePoolLock = new object();
    float lastProcessTime;
    private async Task SpawnTiles()
    {
        // if (Time.time < lastProcessTime + .01f)
        // {
        //     return;
        // }

        bool cacheMiss = false;

        if (spawnedMapLowY < targetMapLowY || (spawnedMapLowY == targetMapLowY && spawnedMapLowX < targetMapLowX))
        {
            spawnedMapLowX += 1;
            if (spawnedMapLowX >= NUM_SEGMENTS_SPAWNED_WIDTH)
            {
                spawnedMapLowY += 1;
                spawnedMapLowX = 0;
            }

            GameObject tile;
            lock (tilePoolLock)
            {
                tile = this.mapTiles.First.Value;
                this.mapTiles.RemoveFirst();
                this.mapTiles.AddLast(tile);
            }
            cacheMiss = await formatSegment(new Vector2Int(spawnedMapLowX, spawnedMapLowY + NUM_SEGMENTS_SPAWNED_HEIGHT), tile);
        }

        if (spawnedMapLowY > targetMapLowY || (spawnedMapLowY == targetMapLowY && spawnedMapLowX > targetMapLowX))
        {
            spawnedMapLowX -= 1;
            if (spawnedMapLowX < 0)
            {
                spawnedMapLowX = NUM_SEGMENTS_SPAWNED_WIDTH - 1;
                spawnedMapLowY -= 1;
            }

            GameObject tile;
            lock (tilePoolLock)
            {
                tile = this.mapTiles.Last.Value;
                this.mapTiles.RemoveLast();
                this.mapTiles.AddFirst(tile);
            }

            cacheMiss = await formatSegment(new Vector2Int(spawnedMapLowX, spawnedMapLowY), tile);
        }

        if (cacheMiss)
        {
            lastProcessTime = Time.time;
        }
    }

    private GameObject createSegment()
    {
        return Instantiate(Tile, Vector3.one * 10000, Tile.transform.rotation, this.transform);
    }

    private async Task<bool> formatSegment(Vector2Int pos, GameObject segment)
    {
        segment.name = $"Map Segment {pos.x},{pos.y}";
        segment.transform.position = new Vector3(tileWidth * pos.x, 0, tileWidth * pos.y);
        Vector2Int gridPos = new Vector2Int(pos.x, pos.y);
        bool cacheMiss = false;
        if (mapCache.ContainsKey(gridPos) == false)
        {
            IslandGenerator.MapPoint[,] map;
            if (pos.x == NUM_SEGMENTS_SPAWNED_WIDTH - 1)
            {
                map = await generator.GetSegment(pos.x, pos.y, Seed, -.5f, 1);
            }
            else if (pos.x == 0)
            {
                map = await generator.GetSegment(pos.x, pos.y, Seed, .5f, .5f);
            }
            else
            {
                map = await generator.GetSegment(pos.x, pos.y, Seed);
            }

            mapCache[gridPos] = new MapSegment
            {
                Map = map,
                Texture = generator.GetTextureOfMap(map),
            };
            cacheMiss = true;
        }

        segment.GetComponent<MeshRenderer>().material.mainTexture = mapCache[gridPos].Texture;
        return cacheMiss;
    }
}

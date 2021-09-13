using System.Collections.Generic;
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
    private Camera cam;
    private bool initialized;
    private Dictionary<Vector2Int, GameObject> activeTiles;
    private System.Random Random = new System.Random(Seed);
    private OverworldSegment island;
    private const float ISLAND_WORLD_SIZE = 20;

    private enum ObjectType
    {
        Tile = 0,
        City = 1,
    }

    // Start is called before the first frame update
    void Start()
    {
        generator = this.transform.GetComponent<OverworldTerrainGenerator>();
        generator.Initialize(0);
        cam = Camera.main;
        activeTiles = new Dictionary<Vector2Int, GameObject>();
        Fortresses = new List<OverworldFortress>();

        pool = new Pool(
            new Dictionary<int, GameObject>
            {
                {
                    (int)ObjectType.City, City
                }
            }
        );

        SpawnIsland();
        CreateTerritoryTexture();
    }

    private void SpawnIsland()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Generate Terrain");
        this.island = generator.GetSegment(0);
        UnityEngine.Profiling.Profiler.EndSample();
        GameObject tile = Instantiate(this.Tile, Vector3.zero, Tile.transform.rotation);
        UnityEngine.Profiling.Profiler.BeginSample("Generate Texture");
        tile.GetComponent<Renderer>().material.mainTexture = generator.GetTextureOfMap(this.island.Points);
        tile.transform.localScale = Vector3.one * ISLAND_WORLD_SIZE;
        UnityEngine.Profiling.Profiler.EndSample();

        foreach (Vector2Int fortressPos in this.island.Fortresses)
        {
            SpawnFortress(tile, fortressPos);
        }
    }

    const float expectedCitiesPerRow = .5f;
    const float powerGainedPerCity = .1f;
    private void SpawnFortress(GameObject tile, Vector2Int gridPos)
    {
        float variance = 1 + (float)Random.NextDouble() / 2 - .25f;
        // float powerMultiplier = Mathf.Pow(1 + powerGainedPerCity * expectedCitiesPerRow, gridPos.y) * variance;
        float powerMultiplier = UnityEngine.Random.Range(.5f, 3f);

        GameObject fortress = pool.GetObject((int)ObjectType.City);
        fortress.transform.position = MapPointToWorldPoint(gridPos);
        fortress.transform.parent = tile.transform;
        OverworldFortress fortressScript = fortress.GetComponent<OverworldFortress>();
        Fortresses.Add(fortressScript);
        fortressScript.Setup(powerMultiplier, gridPos);
        fortressScript.CreateLinks();
    }

    private Dictionary<OverworldFortress, OverworldTerritory> CalculateTerritories()
    {
        int dimensions = OverworldTerrainGenerator.DIMENSIONS;
        List<Vector2Int> points = new List<Vector2Int>();
        foreach (OverworldFortress fortress in Fortresses)
        {
            points.Add(fortress.Position);
            fortress.Alliance = Alliances.Maltov;
        }

        Fortresses[0].Alliance = Alliances.Player;

        bool[,] visited = new bool[dimensions, dimensions];
        var territoryPoints = new Dictionary<OverworldFortress, List<Vector2Int>>();
        var queues = new Dictionary<OverworldFortress, Queue<Vector2Int>>();
        foreach (OverworldFortress fortress in Fortresses)
        {
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(fortress.Position);
            queues[fortress] = queue;
        }

        List<OverworldFortress> finishedFortresses = new List<OverworldFortress>(0);
        while (queues.Count > 0)
        {
            foreach (OverworldFortress fortress in queues.Keys)
            {
                var queue = queues[fortress];

                if (queue.Count == 0)
                {
                    finishedFortresses.Add(fortress);
                    continue;
                }

                Vector2Int current = queue.Dequeue();
                if (visited[current.x, current.y])
                {
                    continue;
                }
                visited[current.x, current.y] = true;
                if (!territoryPoints.ContainsKey(fortress))
                {
                    territoryPoints[fortress] = new List<Vector2Int>();
                }
                territoryPoints[fortress].Add(current);
                foreach (Vector2Int neighbor in Helpers.GetNonHexGridNeighbors(current, dimensions))
                {
                    if (!visited[neighbor.x, neighbor.y])
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }

            foreach (OverworldFortress fortress in finishedFortresses)
            {
                queues.Remove(fortress);
            }
            finishedFortresses = new List<OverworldFortress>();
        }

        var territories = new Dictionary<OverworldFortress, OverworldTerritory>();
        foreach (OverworldFortress fortress in territoryPoints.Keys)
        {
            territories[fortress] = new OverworldTerritory();
            territories[fortress].Points = territoryPoints[fortress];
        }

        return territories;
    }

    private Dictionary<Alliances, Color> allianceColorMap = new Dictionary<Alliances, Color>
    {
        {Alliances.Maltov, ColorExtensions.Create("#ff4633", 50)},
        {Alliances.Player, ColorExtensions.Create("#ffdf5a", 50)},
        {Alliances.Neutral, new Color(0, 0, 0, 0)}
    };
    private void CreateTerritoryTexture()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Generate Territory");
        Dictionary<OverworldFortress, OverworldTerritory> territories = CalculateTerritories();

        foreach (OverworldFortress fortress in territories.Keys)
        {
            OverworldTerritory territory = territories[fortress];
            Texture2D texture = new Texture2D(
                territory.Size.x,
                territory.Size.y,
                TextureFormat.ARGB32,
                false);
            texture.filterMode = FilterMode.Point;

            for (int x = 0; x < territory.Size.x; x++)
            {
                for (int y = 0; y < territory.Size.y; y++)
                {
                    texture.SetPixel(x, y, allianceColorMap[Alliances.Neutral]);
                }
            }

            foreach (Vector2Int point in territory.Points)
            {
                if (this.island.Points[point.x, point.y].Biome != Biome.Water)
                {
                    texture.SetPixel(
                        point.x - territory.LowBounds.x,
                        point.y - territory.LowBounds.y,
                        allianceColorMap[fortress.Alliance]);
                }
            }

            texture.Apply();
            GameObject territoryObject = Instantiate(
                this.Tile,
                Vector3.zero,
                this.Tile.transform.rotation);
            territoryObject.GetComponent<Renderer>().material.mainTexture = texture;
            territoryObject.GetComponent<Renderer>().material.renderQueue = 3002;
            territoryObject.transform.localScale = new Vector3(
                ((float)territory.Size.x / (float)OverworldTerrainGenerator.DIMENSIONS) * (float)ISLAND_WORLD_SIZE,
                ((float)territory.Size.y / (float)OverworldTerrainGenerator.DIMENSIONS) * (float)ISLAND_WORLD_SIZE,
                .01f
            );
            territoryObject.transform.position = MapPointToWorldPoint(territory.Center);
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    private Vector3 MapPointToWorldPoint(Vector2Int mapPoint)
    {
        return MapPointToWorldPoint(new Vector2(mapPoint.x, mapPoint.y));
    }

    private Vector3 MapPointToWorldPoint(Vector2 mapPoint)
    {
        Vector3 offset = ((mapPoint / OverworldTerrainGenerator.DIMENSIONS) - new Vector2(.5f, .5f)) * ISLAND_WORLD_SIZE;
        offset.z = offset.y;
        offset.y = 0;
        return offset;
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
}

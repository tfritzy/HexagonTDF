using System.Collections;
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
    public GameObject LoadingWindowPrefab;

    private Pool pool;
    private Camera cam;
    private bool initialized;
    private Dictionary<Vector2Int, GameObject> activeTiles;
    private System.Random Random = new System.Random(Seed);
    private OverworldSegment island;
    private const float ISLAND_WORLD_SIZE = 20;
    private LoadingWindow loadingWindow;
    private Dictionary<OverworldFortress, OverworldTerritory> territories;

    private enum ObjectType
    {
        Tile = 0,
        City = 1,
    }

    void Awake()
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
        this.loadingWindow = Instantiate(
            LoadingWindowPrefab,
            Managers.LoadingCanvas.transform.position,
            new Quaternion(),
            Managers.LoadingCanvas.transform)
                .GetComponent<LoadingWindow>();

        StartCoroutine("SpawnIsland");
    }

    void FixedUpdate()
    {
        if (this.loadingWindow != null && !this.generator.IsComplete)
        {
            this.loadingWindow.SetStatus(generator.GenerationStep, generator.GenerationProgress);
        }
    }

    private IEnumerator SpawnIsland()
    {
        yield return generator.GenerateSegment(0);
        this.island = generator.Segment;
        GameObject tile = Instantiate(this.Tile, Vector3.zero, Tile.transform.rotation);
        tile.GetComponent<Renderer>().material.mainTexture = generator.Texture;

        tile.transform.localScale = Vector3.one * ISLAND_WORLD_SIZE;

        foreach (Vector2Int fortressPos in this.island.FortressPositions.Values)
        {
            SpawnFortress(tile, fortressPos);
        }

        SpawnTerritories();

        Destroy(this.loadingWindow.gameObject);
        StopCoroutine("SpawnIsland");
    }

    private void SpawnTerritories()
    {
        bool first = true;
        foreach (OverworldTerritory territory in this.island.Territories.Values)
        {
            GameObject territoryObject = Instantiate(
                this.Tile,
                Vector3.zero,
                this.Tile.transform.rotation);
            territoryObject.GetComponent<Renderer>().material.mainTexture = territory.Texture;
            territoryObject.GetComponent<Renderer>().material.renderQueue = 3002;
            territoryObject.GetComponent<Renderer>().material.color = first ? allianceColorMap[Alliances.Player] : allianceColorMap[Alliances.Maltov];
            territoryObject.transform.localScale = new Vector3(
                ((float)territory.Size.x / (float)OverworldTerrainGenerator.DIMENSIONS) * (float)ISLAND_WORLD_SIZE,
                ((float)territory.Size.y / (float)OverworldTerrainGenerator.DIMENSIONS) * (float)ISLAND_WORLD_SIZE,
                .01f
            );
            territoryObject.transform.position = MapPointToWorldPoint(territory.Center);
            first = false;
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
    }

    private Dictionary<Alliances, Color> allianceColorMap = new Dictionary<Alliances, Color>
    {
        {Alliances.Maltov, ColorExtensions.Create("#77403b")},
        {Alliances.Player, ColorExtensions.Create("#ffdf5a")},
        {Alliances.Neutral, new Color(0, 0, 0, 0)}
    };

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

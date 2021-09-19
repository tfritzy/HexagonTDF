using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
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
            this.loadingWindow.SetStatus(generator.GetStatus(), generator.GetProgress());
        }
    }

    private IEnumerator SpawnIsland()
    {
        if (MeshPersister.TryGetCacheItem("Islands/island_0", out Mesh mesh))
        {
            GameObject map = new GameObject();
            map.transform.SetParent(this.transform);
            map.transform.rotation = this.transform.rotation;
            MeshFilter filter = map.AddComponent<MeshFilter>();
            filter.mesh = mesh;
            MeshRenderer renderer = map.AddComponent<MeshRenderer>();
            renderer.material = Constants.Materials.OverworldColorPalette;
        }
        else
        {
            yield return generator.GenerateSegment(0, "Overworld Map");
            this.island = generator.Segment;

            var map = this.transform.Find("Overworld Map");
            MeshPersister.CacheItem("Islands", "island_0", map.GetComponent<MeshFilter>().mesh);
        }


        foreach (Vector2Int fortressPos in this.island.FortressPositions.Values)
        {
            SpawnFortress(fortressPos);
        }

        // SpawnTerritories();

        Destroy(this.loadingWindow.gameObject);
        StopCoroutine("SpawnIsland");
    }

    const float expectedCitiesPerRow = .5f;
    const float powerGainedPerCity = .1f;
    private void SpawnFortress(Vector2Int gridPos)
    {
        float variance = 1 + (float)Random.NextDouble() / 2 - .25f;
        // float powerMultiplier = Mathf.Pow(1 + powerGainedPerCity * expectedCitiesPerRow, gridPos.y) * variance;
        float powerMultiplier = UnityEngine.Random.Range(.5f, 3f);

        GameObject fortress = pool.GetObject((int)ObjectType.City);
        fortress.transform.position = Helpers.ToOverworldPosition(gridPos);
        fortress.transform.parent = this.transform;
        OverworldFortress fortressScript = fortress.GetComponent<OverworldFortress>();
        Fortresses.Add(fortressScript);
        fortressScript.Setup(powerMultiplier, gridPos);
    }

    private Dictionary<Alliances, Color> allianceColorMap = new Dictionary<Alliances, Color>
    {
        {Alliances.Maltov, ColorExtensions.Create("FF8181")},
        {Alliances.Player, ColorExtensions.Create("#ffdf5a")},
        {Alliances.Neutral, new Color(0, 0, 0, 0)}
    };

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

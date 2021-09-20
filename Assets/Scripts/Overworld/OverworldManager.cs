using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public OverworldSegment Island;

    private Pool pool;
    private Camera cam;
    private bool initialized;
    private Dictionary<Vector2Int, GameObject> activeTiles;
    private System.Random Random = new System.Random(Seed);
    private LoadingWindow loadingWindow;
    private Dictionary<OverworldFortress, OverworldTerritory> territories;

    private enum ObjectType
    {
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

        StartCoroutine("SpawnIsland", 0);
    }

    void FixedUpdate()
    {
        if (this.loadingWindow != null && !this.generator.IsComplete)
        {
            this.loadingWindow.SetStatus(generator.GetStatus(), generator.GetProgress());
        }
    }

    const string ISLANDS_FILE_PATH = "Islands";
    const string ISLANDS_MESH_FILE_FORMAT = "island_mesh_{0}";
    const string ISLANDS_DATA_FILE_FORMAT = "island_data_{0}";
    const string MESH_OBJECT_NAME = "Overworld Map Mesh";
    private IEnumerator SpawnIsland(int index)
    {
        if (MeshPersister.TryGetCacheMesh(
                Path.Combine(ISLANDS_FILE_PATH, string.Format(ISLANDS_MESH_FILE_FORMAT, index.ToString())),
                out Mesh cachedMesh) &&
            MeshPersister.TryGetDataFromCache<OverworldSegment>(
                Path.Combine(ISLANDS_FILE_PATH, string.Format(ISLANDS_DATA_FILE_FORMAT, index.ToString())),
                out OverworldSegment cachedSegment))
        {
            GameObject map = new GameObject();
            map.transform.SetParent(this.transform);
            map.transform.rotation = this.transform.rotation;
            MeshFilter filter = map.AddComponent<MeshFilter>();
            filter.mesh = cachedMesh;
            MeshRenderer renderer = map.AddComponent<MeshRenderer>();
            renderer.material = Constants.Materials.OverworldColorPalette;
            this.Island = cachedSegment;
        }
        else
        {
            this.loadingWindow = Instantiate(
                LoadingWindowPrefab,
                Managers.LoadingCanvas.transform.position,
                new Quaternion(),
                Managers.LoadingCanvas.transform)
                    .GetComponent<LoadingWindow>();

            yield return generator.GenerateSegment(0, MESH_OBJECT_NAME);
            this.Island = generator.Segment;
            MeshPersister.CacheData<OverworldSegment>(
                ISLANDS_FILE_PATH,
                string.Format(ISLANDS_DATA_FILE_FORMAT, index.ToString()),
                this.Island);
            var map = this.transform.Find(MESH_OBJECT_NAME);
            MeshPersister.CacheMesh(
                ISLANDS_FILE_PATH,
                string.Format(ISLANDS_MESH_FILE_FORMAT, index.ToString()),
                map.GetComponent<MeshFilter>().mesh);
            Destroy(this.loadingWindow.gameObject);
        }

        foreach (Vector2Int fortressPos in this.Island.FortressPositions.Values)
        {
            SpawnFortress(fortressPos);
        }

        // SpawnTerritories();
        StopCoroutine("SpawnIsland");
    }

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
}

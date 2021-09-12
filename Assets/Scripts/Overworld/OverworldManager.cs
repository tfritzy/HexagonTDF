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
    private float tileWidth;

    private enum ObjectType
    {
        Tile = 0,
        City = 1,
    }

    // Start is called before the first frame update
    async void Start()
    {
        generator = this.transform.GetComponent<OverworldTerrainGenerator>();
        cam = Camera.main;
        activeTiles = new Dictionary<Vector2Int, GameObject>();
        Fortresses = new List<OverworldFortress>();
        tileWidth = Tile.GetComponent<MeshRenderer>().bounds.extents.x * 2;

        pool = new Pool(
            new Dictionary<int, GameObject>
            {
                {
                    (int)ObjectType.City, City
                }
            }
        );

        SpawnIsland();
    }

    private async void SpawnIsland()
    {
        OverworldSegment segment = await generator.GetSegment(0, 0);
        GameObject tile = Instantiate(this.Tile, Vector3.zero, Tile.transform.rotation);
        tile.GetComponent<Renderer>().material.mainTexture = generator.GetTextureOfMap(segment.Points);

        foreach (Vector2Int fortressPos in segment.Fortresses)
        {
            SpawnFortress(tile, fortressPos, segment.Points.GetLength(0));
        }
    }

    const float expectedCitiesPerRow = .5f;
    const float powerGainedPerCity = .1f;
    private void SpawnFortress(GameObject tile, Vector2Int gridPos, float mapDimensions)
    {
        float variance = 1 + (float)Random.NextDouble() / 2 - .25f;
        float powerMultiplier = Mathf.Pow(1 + powerGainedPerCity * expectedCitiesPerRow, gridPos.y) * variance;
        Vector2 floatPos = new Vector2(gridPos.x, gridPos.y);

        GameObject fortress = pool.GetObject((int)ObjectType.City);
        Vector3 fortressOffset = ((floatPos / mapDimensions) - new Vector2(.5f, .5f)) * tileWidth;
        fortressOffset.z = fortressOffset.y;
        fortressOffset.y = 0;
        fortress.transform.position = tile.transform.position + fortressOffset;
        fortress.transform.parent = tile.transform;
        OverworldFortress fortressScript = fortress.GetComponent<OverworldFortress>();
        Fortresses.Add(fortressScript);
        fortressScript.Setup(powerMultiplier, gridPos);
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
}

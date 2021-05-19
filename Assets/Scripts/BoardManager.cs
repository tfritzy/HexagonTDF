using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public const int BoardWidth = 16;
    public const int BoardHeight = 15;
    public const int IslandRadius = 6;
    public HexagonMono[,] Hexagons;
    public Dictionary<Vector2Int, Building> Buildings;
    public Building Source;
    public string ActiveMapName;
    public bool RegenerateMap;
    public Map Map;

    void Awake()
    {
        SpawnMap();
    }

    void Update()
    {
        if (RegenerateMap)
        {
            CleanupMap();
            SpawnMap();
            RegenerateMap = false;
        }
    }

    private void SpawnMap()
    {
        Map = GenerateMap(BoardWidth, BoardHeight);
        SpawnHexagons(Map);
        SpawnBuildings(Map.Buildings);
    }

    private void CleanupMap()
    {
        foreach (HexagonMono hexagon in this.Hexagons)
        {
            Destroy(hexagon?.gameObject);
        }
    }

    private void SpawnHexagons(Map map)
    {
        this.Hexagons = new HexagonMono[map.Width, map.Height];

        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                if (map.GetHex(x, y) == null)
                {
                    continue;
                }

                BuildHexagon(map.GetHex(x, y).Value, x, y);
            }
        }

        Managers.EnemySpawner.SetShoreHexes(this.Map.LandableShores);
    }

    public void AddBuilding(Building building)
    {
        if (Buildings.ContainsKey(building.GridPosition) && Buildings[building.GridPosition] != null)
        {
            throw new System.ArgumentException("Cannot build a building on an occupied spot.");
        }

        Buildings[building.GridPosition] = building;

        GiveShoresPaths();
    }

    private void SpawnBuildings(Dictionary<Vector2Int, BuildingType> buildingMap)
    {
        this.Buildings = new Dictionary<Vector2Int, Building>();

        // TODO: Remove hack and let player choose source position.
        Vector2Int sourcePos = this.Map.MainLandmass.ToList().First();
        buildingMap[sourcePos] = BuildingType.Source;

        foreach (Vector2Int pos in buildingMap.Keys)
        {
            Building building = Instantiate(
                    Prefabs.Buildings[buildingMap[pos]],
                    Map.ToWorldPosition(pos),
                    new Quaternion(),
                    this.transform)
                    .GetComponent<Building>();

            if (building.Type == BuildingType.Source)
            {
                Source = building;
            }

            building.Initialize(pos);
        }
    }

    private void GiveShoresPaths()
    {
        Vector2Int[,] predGrid = Helpers.GetPredecessorGrid(this.Map, this.Source.GridPosition, Helpers.IsTraversable);

        foreach (Vector2Int pos in Map.LandableShores)
        {
            List<Vector2Int> newPath = Helpers.FindPath(predGrid, this.Source.GridPosition, pos);
            Hexagons[pos.x, pos.y].GetComponent<ShoreMono>().SetPath(newPath);
        }
    }

    public HexagonType?[,] GetTypeMap()
    {
        HexagonType?[,] typeMap = new HexagonType?[this.Hexagons.GetLength(0), this.Hexagons.GetLength(1)];
        for (int x = 0; x < this.Hexagons.GetLength(0); x++)
        {
            for (int y = 0; y < this.Hexagons.GetLength(1); y++)
            {
                typeMap[x, y] = this.Hexagons[x, y]?.Type;
            }
        }

        return typeMap;
    }

    public const float LAND_PERLIN_SCALE = 5f;
    public const float FORREST_PERLIN_SCALE = 3f;
    public const float LandPerlinCutoff = .65f;
    public const float TreePerlinCutoff = .55f;
    public Map GenerateMap(int width, int height)
    {
        Map newMap = new Map();
        HexagonType?[,] hexes = new HexagonType?[width, height];
        int seed = Random.Range(0, 100000);
        int forrestSeed = Random.Range(0, 100000);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (DistFromCenter(x, y) > IslandRadius)
                {
                    hexes[x, y] = HexagonType.Water;
                    continue;
                }

                float sampleX = x / LAND_PERLIN_SCALE;
                float sampleY = y / LAND_PERLIN_SCALE;
                float perlinValue = Mathf.PerlinNoise(sampleX + seed, sampleY + seed);
                hexes[x, y] = perlinValue < LandPerlinCutoff ? HexagonType.Grass : HexagonType.Water;

                float treePerlinValue = Mathf.PerlinNoise(x / FORREST_PERLIN_SCALE + forrestSeed, y / FORREST_PERLIN_SCALE + forrestSeed);
                if (treePerlinValue > TreePerlinCutoff)
                {
                    hexes[x, y] = HexagonType.Forrest;
                }
            }
        }

        newMap.SetHexes(hexes);
        return newMap;
    }

    private static float DistFromCenter(int x, int y)
    {
        Vector2 vector = new Vector2(x - BoardWidth / 2, y - BoardHeight / 2);
        return vector.magnitude;
    }

    public bool IsBlockedByBuilding(Vector2Int gridPosition)
    {
        return Buildings.ContainsKey(gridPosition) && Buildings[gridPosition] != null;
    }

    public bool IsBuildable(Vector2Int pos)
    {
        return Hexagons[pos.x, pos.y].IsBuildable && Buildings.ContainsKey(pos) == false;
    }

    public Dictionary<Vector2Int, BuildingType> GetBuildingTypeMap()
    {
        Dictionary<Vector2Int, BuildingType> typeMap = new Dictionary<Vector2Int, BuildingType>();
        foreach (Vector2Int key in Buildings.Keys)
        {
            typeMap[key] = Buildings[key].Type;
        }

        return typeMap;
    }

    private void BuildHexagon(HexagonType type, int x, int y)
    {
        GameObject go = Instantiate(Prefabs.Hexagons[type], Map.ToWorldPosition(x, y), new Quaternion(), this.transform);
        HexagonMono hexagonScript = go.GetComponent<HexagonMono>();
        hexagonScript.SetType(Prefabs.GetHexagonScript(type));
        this.Hexagons[x, y] = go.GetComponent<HexagonMono>();
        this.Hexagons[x, y].GridPosition = new Vector2Int(x, y);
    }
}

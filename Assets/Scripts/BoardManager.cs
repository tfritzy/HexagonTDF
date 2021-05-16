using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject HexagonPrefab;
    public const int BoardWidth = 16;
    public const int BoardHeight = 15;
    public const int IslandRadius = 6;
    public Hexagon[,] Hexagons;
    public Dictionary<Vector2Int, Building> Buildings;
    public List<Portal> Portals;
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
        // Map map = LoadMap();
        Map = GenerateMap(BoardWidth, BoardHeight);
        SpawnHexagons(Map);
        SpawnBuildings(Map.Buildings);
    }

    private void CleanupMap()
    {
        foreach (Hexagon hexagon in this.Hexagons)
        {
            Destroy(hexagon?.gameObject);
        }
    }

    private void SpawnHexagons(Map map)
    {
        this.Hexagons = new Hexagon[map.Width, map.Height];

        for (int y = 0; y < map.Width; y++)
        {
            for (int x = 0; x < map.Height; x++)
            {
                if (map.GetHex(x, y) == null)
                {
                    continue;
                }

                BuildHexagon(map.GetHex(x, y).Value, x, y);
            }
        }
    }

    public void AddBuilding(Building building)
    {
        if (Buildings.ContainsKey(building.Position) && Buildings[building.Position] != null)
        {
            throw new System.ArgumentException("Cannot build a building on an occupied spot.");
        }

        Buildings[building.Position] = building;

        foreach (Portal portal in Portals)
        {
            portal.RecalculatePath();
        }
    }

    private void SpawnBuildings(Dictionary<string, BuildingType> buildingMap)
    {
        this.Buildings = new Dictionary<Vector2Int, Building>();
        foreach (string strPosition in buildingMap.Keys)
        {
            string[] posSplits = strPosition.Split(',');
            Vector2Int position = new Vector2Int(int.Parse(posSplits[0]), int.Parse(posSplits[1]));
            Building building = Instantiate(
                    Prefabs.Buildings[buildingMap[strPosition]],
                    Map.ToWorldPosition(position),
                    new Quaternion(),
                    this.transform)
                    .GetComponent<Building>();
            building.Initialize(position);

            if (buildingMap[strPosition] == BuildingType.Source)
            {
                Source = this.Buildings[position];
            }

            if (buildingMap[strPosition] == BuildingType.Portal)
            {
                Portals.Add((Portal)building);
            }
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

    private Map LoadMap()
    {
        TextAsset text = Resources.Load<TextAsset>(Constants.FilePaths.Maps + ActiveMapName);
        if (text == null)
        {
            Map newMap = new Map();
            HexagonType?[,] hexes = new HexagonType?[BoardWidth, BoardHeight];
            for (int i = 0; i < hexes.GetLength(0); i++)
            {
                for (int j = 0; j < hexes.GetLength(1); j++)
                {
                    hexes[i, j] = HexagonType.Grass;
                }
            }

            newMap.SetHexes(hexes);
            return newMap;
        }

        Map map = JsonConvert.DeserializeObject<Map>(text.text);
        return map;
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
        GameObject go = Instantiate(HexagonPrefab, Map.ToWorldPosition(x, y), new Quaternion(), this.transform);
        Hexagon hexagonScript = Prefabs.GetHexagonScript(type);
        go.AddComponent(hexagonScript.GetType());
        this.Hexagons[x, y] = go.GetComponent<Hexagon>();
        this.Hexagons[x, y].GridPosition = new Vector2Int(x, y);
    }
}

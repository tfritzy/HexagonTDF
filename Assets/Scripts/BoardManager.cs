using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public const int BoardWidth = 20;
    public const int BoardHeight = 22;
    public const int IslandRadius = 8;
    public HexagonMono[,] Hexagons;
    public Dictionary<Vector2Int, Building> Buildings;
    public List<Orb> Orbs;
    public string ActiveMapName;
    public bool RegenerateMap;
    public Map Map;
    public Guid PathingId { get; private set; }
    public Dictionary<Vector2Int, Character> CharacterPositions;
    private Dictionary<Vector2Int, Vector2Int[,]> predGridMap;
    private Dictionary<Vector2Int, Vector2Int[,]> predGridIgnoringBuildings;

    void Awake()
    {
        CharacterPositions = new Dictionary<Vector2Int, Character>();
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
        Map = new Map(BoardWidth, BoardHeight, IslandRadius);
        SpawnHexagons(Map);
        SpawnBuildings(Map.Buildings);
        CalculatePredGridIgnoringBuildings();
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

        foreach (Vector2Int pos in map.OceanHex)
        {
            this.Hexagons[pos.x, pos.y].transform.Find("Hex").gameObject.SetActive(false);
        }

        // Managers.EnemySpawner.SetShoreHexes(this.Map.LandableShores);
    }

    public void AddBuilding(Building building)
    {
        if (Buildings.ContainsKey(building.GridPosition) && Buildings[building.GridPosition] != null)
        {
            throw new System.ArgumentException("Cannot build a building on an occupied spot.");
        }

        Buildings[building.GridPosition] = building;

        RecalculatePredGrid();
    }

    private void SpawnBuildings(Dictionary<Vector2Int, BuildingType> buildingMap)
    {
        this.Buildings = new Dictionary<Vector2Int, Building>();

        foreach (Vector2Int pos in buildingMap.Keys)
        {
            Building building = Instantiate(
                    Prefabs.Buildings[buildingMap[pos]],
                    Hexagons[pos.x, pos.y].transform.position,
                    new Quaternion(),
                    this.transform)
                    .GetComponent<Building>();

            if (building is Orb)
            {
                Orbs.Add((Orb)building);
            }

            building.Initialize(pos);
        }
    }

    private bool isValidPath(List<Vector2Int> path, Vector2Int expectedStart, Vector2Int expectedEnd)
    {
        return path?.Count > 0 && path[0] == expectedStart && path.Last() == expectedEnd;
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

    public Vector2Int GetNextStepInPathToSource(Vector2Int sourceBuilding, Vector2Int currentPos, bool ignoreBuildings = false)
    {
        if (ignoreBuildings)
        {
            return predGridIgnoringBuildings[sourceBuilding][currentPos.x, currentPos.y];
        }

        return predGridMap[sourceBuilding][currentPos.x, currentPos.y];
    }

    private void RecalculatePredGrid()
    {
        predGridMap = new Dictionary<Vector2Int, Vector2Int[,]>();

        foreach (Orb orb in Orbs)
        {
            predGridMap[orb.GridPosition] = Helpers.GetPredecessorGrid(
                this.Map,
                orb.GridPosition,
                (Vector2Int pos) =>
                {
                    return Helpers.IsTraversable(pos) || pos == orb.GridPosition || (Buildings.ContainsKey(pos) && Buildings[pos].Type == BuildingType.Dock);
                });
        }
    }

    private void CalculatePredGridIgnoringBuildings()
    {
        predGridIgnoringBuildings = new Dictionary<Vector2Int, Vector2Int[,]>();

        foreach (Orb orb in Orbs)
        {
            predGridIgnoringBuildings[orb.GridPosition] = Helpers.GetPredecessorGrid(
                this.Map,
                orb.GridPosition,
                (Vector2Int pos) =>
                {
                    return Managers.Board.Hexagons[pos.x, pos.y].IsWalkable || (Buildings.ContainsKey(pos) && Buildings[pos].Type == BuildingType.Dock);
                });
        }
    }

    public bool IsBuildable(Vector2Int pos)
    {
        return Hexagons[pos.x, pos.y].IsBuildable && Buildings.ContainsKey(pos) == false;
    }

    private void BuildHexagon(HexagonType type, int x, int y)
    {
        Vector3 position = Map.ToWorldPosition(x, y);
        position.y = Map.HexHeightMap[x, y];
        GameObject go = Instantiate(Prefabs.Hexagons[type], position, new Quaternion(), this.transform);
        HexagonMono hexagonScript = go.GetComponent<HexagonMono>();
        hexagonScript.SetType(Prefabs.GetHexagonScript(type));
        this.Hexagons[x, y] = go.GetComponent<HexagonMono>();
        this.Hexagons[x, y].GridPosition = new Vector2Int(x, y);
    }

    public HexagonMono GetHex(Vector2Int pos)
    {
        if (Helpers.IsInBounds(this.Map, pos) == false)
        {
            return null;
        }

        return Hexagons[pos.x, pos.y];
    }
}

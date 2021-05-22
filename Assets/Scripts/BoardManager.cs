using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public const int BoardWidth = 20;
    public const int BoardHeight = 22;
    public const int IslandRadius = 5;
    public HexagonMono[,] Hexagons;
    public Dictionary<Vector2Int, Building> Buildings;
    public Building Source;
    public string ActiveMapName;
    public bool RegenerateMap;
    public Map Map;
    public Guid PathingId { get; private set; }

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
        Map = new Map(BoardWidth, BoardHeight, IslandRadius);
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

        RecalculatePredGrid();
    }

    private void SpawnBuildings(Dictionary<Vector2Int, BuildingType> buildingMap)
    {
        this.Buildings = new Dictionary<Vector2Int, Building>();

        // TODO: Remove hack and let player choose source position.
        List<Vector2Int> mainLandmass = this.Map.MainLandmass.ToList();
        Vector2Int sourcePos = mainLandmass[UnityEngine.Random.Range(0, mainLandmass.Count)];
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

    Vector2Int[,] predGrid;
    public Vector2Int GetNextStepInPathToSource(Vector2Int currentPos)
    {
        return predGrid[currentPos.x, currentPos.y];
    }

    private void RecalculatePredGrid()
    {
        predGrid = Helpers.GetPredecessorGrid(
            this.Map,
            Source.GridPosition,
            (Vector2Int pos) =>
            {
                return Helpers.IsTraversable(pos) || pos == Source.GridPosition;
            });
    }

    public bool IsBuildable(Vector2Int pos)
    {
        return Hexagons[pos.x, pos.y].IsBuildable && Buildings.ContainsKey(pos) == false && ifBlockedWouldNoPathsRemain(pos) == false;
    }

    private bool ifBlockedWouldNoPathsRemain(Vector2Int blockedPos)
    {
        Vector2Int[,] predGrid = Helpers.GetPredecessorGrid(
            this.Map,
            this.Source.GridPosition,
            (Vector2Int testPos) =>
            {
                return testPos != blockedPos && (Helpers.IsTraversable(testPos) || Source.GridPosition == testPos);
            });

        foreach (Vector2Int pos in Map.LandableShores)
        {
            List<Vector2Int> newPath = Helpers.FindPath(predGrid, this.Source.GridPosition, pos);
            newPath.Reverse(); // Because we searched from source out.

            if (newPath[0] != newPath.Last() && newPath[0] == pos && newPath.Last() == this.Source.GridPosition)
            {
                return false;
            }
        }

        return true;
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

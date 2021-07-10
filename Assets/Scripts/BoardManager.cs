﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public const int BoardWidth = 11;
    public const int BoardHeight = 20;
    public HexagonMono[,] Hexagons;
    public Dictionary<Vector2Int, Building> Buildings;
    public Trebuchet Trebuchet;
    public List<Barracks> Barracks;
    public string ActiveMapName;
    public bool RegenerateMap;
    public Map Map;
    public Guid PathingId { get; private set; }
    private Dictionary<Vector2Int, PredGridPoint[,]> predGridMap;
    private Dictionary<Vector2Int, PredGridPoint[,]> flightPredGridMap;

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
        Map = new Map(BoardWidth, BoardHeight);
        SpawnHexagons(Map);
        SpawnTrebuchet(Map);
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

        // foreach (Vector2Int pos in map.OceanHex)
        // {
        //     this.Hexagons[pos.x, pos.y].transform.Find("Hex").gameObject.SetActive(false);
        // }

        // Managers.EnemySpawner.SetShoreHexes(this.Map.LandableShores);
    }

    public void AddBuilding(Building building)
    {
        if (Buildings.ContainsKey(building.GridPosition) && Buildings[building.GridPosition] != null)
        {
            throw new System.ArgumentException("Cannot build a building on an occupied spot.");
        }

        Buildings[building.GridPosition] = building;

        RecalculatePredGrids();
    }

    private void SpawnBuildings(Dictionary<Vector2Int, BuildingType> buildingMap)
    {
        this.Buildings = new Dictionary<Vector2Int, Building>();
        this.Barracks = new List<Barracks>();

        foreach (Vector2Int pos in buildingMap.Keys)
        {
            Building building = Instantiate(
                    Prefabs.Buildings[buildingMap[pos]],
                    Hexagons[pos.x, pos.y].transform.position,
                    new Quaternion(),
                    this.transform)
                    .GetComponent<Building>();

            if (building is Barracks)
            {
                this.Barracks.Add((Barracks)building);
            }

            building.Initialize(pos);
        }
    }

    private void SpawnTrebuchet(Map map)
    {
        Vector2Int pos = map.Trebuchet;
        Trebuchet = Instantiate(
            Prefabs.Allies[AlliedUnitType.Trebuchet],
            Hexagons[pos.x, pos.y].transform.position,
            new Quaternion())
                .GetComponent<Trebuchet>();
        Trebuchet.SetInitialPosition(pos);
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

    public PredGridPoint GetNextStepInPathToSource(Vector2Int sourceBuilding, Vector2Int currentPos)
    {
        return predGridMap[sourceBuilding][currentPos.x, currentPos.y];
    }

    public int GetFlightDistanceToTarget(Vector2Int building, Vector2Int pos)
    {
        return flightPredGridMap[building][pos.x, pos.y].Distance - 1;
    }

    private void RecalculatePredGrids()
    {
        predGridMap = new Dictionary<Vector2Int, PredGridPoint[,]>();

        predGridMap[Trebuchet.GridPosition] = Helpers.GetPredecessorGrid(
            this.Map,
            Trebuchet.GridPosition,
            (Vector2Int prevPos, Vector2Int nextPos) =>
            {
                if ((prevPos != Trebuchet.GridPosition &&
                    Buildings.ContainsKey(prevPos) &&
                    Buildings[prevPos].IsWalkable == false))
                {
                    return false; // You would have needed to pass through a building to get here.
                }

                return Hexagons[nextPos.x, nextPos.y].IsWalkable || nextPos == Trebuchet.GridPosition;
            });


        flightPredGridMap = new Dictionary<Vector2Int, PredGridPoint[,]>();

        flightPredGridMap[Trebuchet.GridPosition] = Helpers.GetPredecessorGrid(
            this.Map,
            Trebuchet.GridPosition,
            (Vector2Int startPos, Vector2Int testEndPos) => true);

    }

    public bool IsBuildable(Vector2Int pos)
    {
        return Hexagons[pos.x, pos.y].IsBuildable && Buildings.ContainsKey(pos) == false;
    }

    private void BuildHexagon(HexagonType type, int x, int y)
    {
        Vector3 position = Helpers.ToWorldPosition(x, y);
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

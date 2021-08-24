using System;
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
    public Guid PathingId { get; private set; }
    public Hero Hero;
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
        SpawnHexagons(GameState.SelectedSegment);
        SpawnTrebuchet(GameState.SelectedSegment);
        SpawnHero(GameState.SelectedSegment);
        SpawnBuildings(GameState.SelectedSegment);
    }

    private void CleanupMap()
    {
        foreach (HexagonMono hexagon in this.Hexagons)
        {
            Destroy(hexagon?.gameObject);
        }
    }

    private void SpawnHexagons(OverworldSegment map)
    {
        this.Hexagons = new HexagonMono[map.Width, map.Height];

        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                if (map.GetPoint(x, y) == null)
                {
                    continue;
                }

                BuildHexagon(map.GetPoint(x, y), x, y);
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

    private void SpawnBuildings(OverworldSegment segment)
    {
        this.Buildings = new Dictionary<Vector2Int, Building>();
        this.Barracks = new List<Barracks>();

        // foreach (Vector2Int pos in buildingMap.Keys)
        // {
        //     Building building = Instantiate(
        //             Prefabs.Buildings[buildingMap[pos]],
        //             Hexagons[pos.x, pos.y].transform.position,
        //             new Quaternion(),
        //             this.transform)
        //             .GetComponent<Building>();

        //     if (building is Barracks)
        //     {
        //         this.Barracks.Add((Barracks)building);
        //     }

        //     building.Initialize(pos);
        // }
    }

    private void SpawnTrebuchet(OverworldSegment map)
    {
        // Vector2Int pos = map.TrebuchetPos;
        // Trebuchet = Instantiate(
        //     Prefabs.Allies[AlliedUnitType.Trebuchet],
        //     Hexagons[pos.x, pos.y].transform.position,
        //     new Quaternion())
        //         .GetComponent<Trebuchet>();
        // Trebuchet.SetInitialPosition(pos);
    }

    private void SpawnHero(OverworldSegment map)
    {
        // Vector2Int pos = map.HeroPos;
        // this.Hero = Instantiate(
        //     Prefabs.Allies[AlliedUnitType.Warrior],
        //     Hexagons[pos.x, pos.y].transform.position,
        //     new Quaternion())
        //         .GetComponent<Hero>();
        // this.Hero.SetInitialPosition(pos);
    }

    private bool isValidPath(List<Vector2Int> path, Vector2Int expectedStart, Vector2Int expectedEnd)
    {
        return path?.Count > 0 && path[0] == expectedStart && path.Last() == expectedEnd;
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
            GameState.SelectedSegment,
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
            GameState.SelectedSegment,
            Trebuchet.GridPosition,
            (Vector2Int startPos, Vector2Int testEndPos) => true);

    }

    public bool IsBuildable(Vector2Int pos)
    {
        return Hexagons[pos.x, pos.y].IsBuildable && Buildings.ContainsKey(pos) == false;
    }

    private void BuildHexagon(OverworldMapPoint point, int x, int y)
    {
        Vector3 position = Helpers.ToWorldPosition(x, y);
        position.y = (point.Height - .5f) * 20;
        GameObject go = Instantiate(Prefabs.Hexagons[point.Biome], position, new Quaternion(), this.transform);
        HexagonMono hexagonScript = go.GetComponent<HexagonMono>();
        hexagonScript.SetType(Prefabs.GetHexagonScript(point.Biome));
        this.Hexagons[x, y] = go.GetComponent<HexagonMono>();
        this.Hexagons[x, y].GridPosition = new Vector2Int(x, y);
    }

    public HexagonMono GetHex(Vector2Int pos)
    {
        if (Helpers.IsInBounds(GameState.SelectedSegment, pos) == false)
        {
            return null;
        }

        return Hexagons[pos.x, pos.y];
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public const int DIMENSIONS = 20;
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
    private Vector2Int xVillageRange = new Vector2Int(DIMENSIONS / 2 - DIMENSIONS / 4, DIMENSIONS / 2 + DIMENSIONS / 4);
    private Vector2Int yVillageRange = new Vector2Int(DIMENSIONS / 2 + DIMENSIONS / 4, DIMENSIONS);

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
        SetupMapHeight(GameState.SelectedSegment);
        FlattenVillageAndTrebArea();
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

    HashSet<Biome> flatBiomes = new HashSet<Biome> { Biome.Water, Biome.Sand, Biome.Grassland, Biome.Snow, Biome.Forrest };
    private void SetupMapHeight(OverworldSegment map)
    {
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(map.Coordinates.GetHashCode());
        for (int y = 0; y < this.Hexagons.GetLength(1); y++)
        {
            for (int x = 0; x < this.Hexagons.GetLength(0); x++)
            {
                Vector3 pos = this.Hexagons[x, y].transform.position;
                pos.y = 0;
                this.Hexagons[x, y].transform.position = pos;

                if (flatBiomes.Contains(this.Hexagons[x, y].Biome))
                {
                    continue;
                }

                double heightVal = heightNoise.Evaluate(x / 5.0f, y / 5.0f, 1) + 1;
                int hieghtModifier = 0;
                if (heightVal > 1.35f) hieghtModifier += 1;
                if (heightVal > 1.6f) hieghtModifier += 1;
                this.Hexagons[x, y].transform.position += hieghtModifier * Vector3.up;
            }
        }
    }

    private void SpawnHexagons(OverworldSegment map)
    {
        this.Hexagons = new HexagonMono[DIMENSIONS, DIMENSIONS];
        int start = map.Height / 2 - DIMENSIONS / 2;
        int end = map.Height / 2 + DIMENSIONS / 2;

        for (int y = start; y < end; y++)
        {
            for (int x = start; x < end; x++)
            {
                if (map.GetPoint(x, y) == null)
                {
                    continue;
                }

                BuildHexagon(map.GetPoint(x, y), x - start, y - start);
            }
        }
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
        System.Random random = new System.Random(segment.Coordinates.GetHashCode());
        var buildingMap = new Dictionary<Vector2Int, BuildingType>();

        Vector2Int castlePosition = new Vector2Int(
            random.Next(xVillageRange.x, xVillageRange.y),
            random.Next(yVillageRange.x, yVillageRange.y)
        );
        buildingMap.Add(castlePosition, BuildingType.Barracks);

        // TODO: Make system for village sizes.
        int numHouses = random.Next(3, 6);
        for (int i = 0; i < numHouses; i++)
        {
            Vector2Int potentialPos;
            do
            {
                potentialPos = new Vector2Int(
                    random.Next(xVillageRange.x, xVillageRange.y),
                    random.Next(yVillageRange.x, yVillageRange.y)
                );
            } while (buildingMap.ContainsKey(potentialPos));

            buildingMap.Add(potentialPos, BuildingType.House);
        }

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

    private void SpawnTrebuchet(OverworldSegment map)
    {
        Vector2Int pos = new Vector2Int(this.Hexagons.GetLength(0) / 2, 2);
        Trebuchet = Instantiate(
            Prefabs.Allies[AlliedUnitType.Trebuchet],
            Hexagons[pos.x, pos.y].transform.position,
            new Quaternion())
                .GetComponent<Trebuchet>();
        Trebuchet.SetInitialPosition(pos);
    }

    private void FlattenVillageAndTrebArea()
    {
        for (int y = this.yVillageRange.x; y < this.yVillageRange.y; y++)
        {
            for (int x = this.xVillageRange.x; x < this.xVillageRange.y; x++)
            {
                Vector3 pos = this.Hexagons[x, y].transform.position;
                pos.y = 0;
                this.Hexagons[x, y].transform.position = pos;
                this.Hexagons[x, y].RemoveObstacle();
            }
        }

        for (int y = 0; y < this.yVillageRange.y / 4; y++)
        {
            for (int x = this.xVillageRange.x; x < this.xVillageRange.y; x++)
            {
                Vector3 pos = this.Hexagons[x, y].transform.position;
                pos.y = 0;
                this.Hexagons[x, y].transform.position = pos;
                this.Hexagons[x, y].RemoveObstacle();
            }
        }
    }

    private void SpawnHero(OverworldSegment map)
    {
        Vector2Int pos = this.Trebuchet.GridPosition + Vector2Int.up;
        this.Hero = Instantiate(
            Prefabs.Allies[AlliedUnitType.Warrior],
            Hexagons[pos.x, pos.y].transform.position,
            new Quaternion())
                .GetComponent<Hero>();
        this.Hero.SetInitialPosition(pos);
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

        predGridMap[Trebuchet.GridPosition] = Helpers.GetPredicessorGridWalking(
            this.Hexagons,
            Trebuchet.GridPosition);


        flightPredGridMap = new Dictionary<Vector2Int, PredGridPoint[,]>();

        flightPredGridMap[Trebuchet.GridPosition] = Helpers.GetPredecessorGrid(
            this.Hexagons,
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
        position.y = point.Height;
        GameObject go = Instantiate(Prefabs.Hexagons[point.Biome], position, new Quaternion(), this.transform);
        HexagonMono hexagonScript = go.GetComponent<HexagonMono>();
        hexagonScript.SetType(Prefabs.GetHexagonScript(point.Biome));
        this.Hexagons[x, y] = go.GetComponent<HexagonMono>();
        this.Hexagons[x, y].GridPosition = new Vector2Int(x, y);
        this.Hexagons[x, y].MaybeSpawnObstacle();
    }

    public HexagonMono GetHex(Vector2Int pos)
    {
        if (Helpers.IsInBounds(this.Hexagons, pos) == false)
        {
            return null;
        }

        return Hexagons[pos.x, pos.y];
    }
}

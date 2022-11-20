using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public HexagonMono[,] Hexagons;
    public Dictionary<Vector2Int, Building> Buildings;
    public Trebuchet Trebuchet;
    public string ActiveMapName;
    public bool RegenerateMap;
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
        OverworldSegment segment;
        if (GameState.SelectedSegment == null)
        {
            segment = OverworldTerrainGenerator.GenerateSingleSegment(25, 35, UnityEngine.Random.Range(0, 10));
        }
        else
        {
            segment = GameState.SelectedSegment;
        }

        SpawnHexagons(segment);
        SetupMapHeight(segment);
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
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(map.Index);
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

    private void SpawnHexagons(OverworldSegment segment)
    {
        this.Hexagons = new HexagonMono[segment.Width, segment.Height];

        for (int y = 0; y < segment.Height; y++)
        {
            for (int x = 0; x < segment.Width; x++)
            {
                if (segment.GetPoint(x, y) == null)
                {
                    continue;
                }

                BuildHexagon(segment.GetPoint(x, y), x, y, segment.Index);
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

    private void BuildHexagon(OverworldMapPoint point, int x, int y, int segmentIndex)
    {
        Vector3 position = Helpers.ToWorldPosition(x, y);
        position.y = point.Height;
        GameObject go = Instantiate(Prefabs.Hexagons[point.Biome], position, new Quaternion(), this.transform);
        HexagonMono hexagonScript = go.GetComponent<HexagonMono>();
        hexagonScript.SetType(Prefabs.GetHexagonScript(point.Biome));
        this.Hexagons[x, y] = go.GetComponent<HexagonMono>();
        this.Hexagons[x, y].GridPosition = new Vector2Int(x, y);
        this.Hexagons[x, y].MaybeSpawnObstacle(segmentIndex);
    }

    public HexagonMono GetHex(Vector2Int pos)
    {
        if (Helpers.IsInBounds(pos, this.Hexagons.GetLength(0)) == false)
        {
            return null;
        }

        return Hexagons[pos.x, pos.y];
    }
}

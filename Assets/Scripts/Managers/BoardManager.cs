using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Hexagon[,] Board;
    public HexagonMono[,] HexagonMonos;
    public string ActiveMapName;
    private Building[,] Buildings;
    public RectInt Dimensions;
    private Vector2Int Center => this.Dimensions.max / 2;
    public Navigation Navigation;

    void Awake()
    {
        SpawnMap();
    }

    private void SpawnMap()
    {
        this.Board = OverworldTerrainGenerator.GenerateSingleSegment(
            Constants.OVERWORLD_DIMENSIONS,
            Constants.OVERWORLD_DIMENSIONS,
            UnityEngine.Random.Range(0, 1000));

        this.Buildings = new Building[Constants.OVERWORLD_DIMENSIONS, Constants.OVERWORLD_DIMENSIONS];
        this.Dimensions = new RectInt(0, 0, Constants.OVERWORLD_DIMENSIONS, Constants.OVERWORLD_DIMENSIONS);

        TownHall townHall = SpawnTownHall();
        Navigation = new Navigation(this.Dimensions.max, townHall);
        AddBuilding(townHall.GridPosition, townHall);
        SpawnHexagons();
        SpawnHero();

        this.Navigation.ReacalculateIdealPath(Board, Buildings);
    }

    private TownHall SpawnTownHall()
    {
        // flatten area
        var points = new List<Vector2Int> { Center };
        for (int i = 0; i < 6; i++)
        {
            points.Add(Helpers.GetNeighborPosition(Center, (HexSide)i));
        }

        float averageHeight = 0;
        List<Biome> biomesThatArentWater = new List<Biome> { Board[Center.x, Center.y].Biome };
        foreach (Vector2Int point in points)
        {
            averageHeight += Board[point.x, point.y].Height;

            if (Board[point.x, point.y].Biome != Biome.Water)
            {
                biomesThatArentWater.Add(Board[point.x, point.y].Biome);
            }
        }

        averageHeight /= 7;
        int newHeight = (int)Mathf.Round(averageHeight);
        Biome mostCommonHex = biomesThatArentWater
            .GroupBy(q => q)
            .OrderByDescending(gp => gp.Count())
            .First().Key;
        foreach (Vector2Int point in points)
        {
            Board[point.x, point.y] = Prefabs.GetHexagonScript(mostCommonHex, newHeight);
        }

        return (TownHall)InstantiateBuilding(Center, BuildingType.TownHall);
    }

    public Building InstantiateBuilding(Vector2Int pos, BuildingType type)
    {
        var buildingGO = GameObject.Instantiate(
            Prefabs.GetBuilding(type),
            Vector3.zero,
            Prefabs.GetBuilding(type).transform.rotation);
        Building building = buildingGO.GetComponent<Building>();
        building.GridPosition = pos;
        buildingGO.transform.position = building.GetWorldPosition();
        return building;
    }

    public LinkedList<Vector2Int> ShortestPathBetween(Vector2Int startPos, Vector2Int endPos)
    {
        return Navigation.BFS(startPos, endPos, this.Board, this.Buildings);
    }

    private void CleanupMap()
    {
        foreach (HexagonMono hexagon in this.HexagonMonos)
        {
            Destroy(hexagon?.gameObject);
        }
    }

    private void SpawnHexagons()
    {
        this.HexagonMonos = new HexagonMono[Board.GetLength(0), Board.GetLength(1)];

        for (int y = 0; y < Board.GetLength(1); y++)
        {
            for (int x = 0; x < Board.GetLength(0); x++)
            {
                if (Board[x, y] == null)
                {
                    continue;
                }

                BuildHexagon(x, y);
            }
        }
    }

    private void SpawnHero()
    {
        GameObject character = GameObject.Instantiate(Prefabs.GetCharacter(CharacterType.MainCharacter));
        character.transform.position = Helpers.ToWorldPosition(Center + new Vector2Int(4, 4));
    }

    private bool isValidPath(List<Vector2Int> path, Vector2Int expectedStart, Vector2Int expectedEnd)
    {
        return path?.Count > 0 && path[0] == expectedStart && path.Last() == expectedEnd;
    }

    private void BuildHexagon(int x, int y)
    {
        Hexagon point = Board[x, y];
        Vector3 position = Helpers.ToWorldPosition(x, y);
        position.y = point.Height * Constants.HEXAGON_HEIGHT;
        GameObject go = Instantiate(Prefabs.Hexagons[point.Biome], position, new Quaternion(), this.transform);
        HexagonMono hexagonScript = go.GetComponent<HexagonMono>();
        hexagonScript.SetType(Prefabs.GetHexagonScript(point.Biome, point.Height));
        this.HexagonMonos[x, y] = go.GetComponent<HexagonMono>();
        this.HexagonMonos[x, y].GridPosition = new Vector2Int(x, y);
        this.HexagonMonos[x, y].Height = point.Height;
        this.HexagonMonos[x, y].name = point.Biome + "," + point.Height.ToString();
        this.HexagonMonos[x, y].SetSideData();
    }

    public HexagonMono GetHex(Vector2Int pos)
    {
        if (Helpers.IsInBounds(pos, this.Dimensions) == false)
        {
            return null;
        }

        return HexagonMonos[pos.x, pos.y];
    }

    public Building GetBuilding(Vector2Int pos)
    {
        if (!Helpers.IsInBounds(pos, this.Dimensions))
        {
            return null;
        }

        return Buildings[pos.x, pos.y];
    }

    public void AddBuilding(Vector2Int pos, Building building)
    {
        this.Buildings[pos.x, pos.y] = building;

        foreach (HexSide side in building.ExtraSize)
        {
            Vector2Int extraPos = Helpers.GetNeighborPosition(pos, side);
            this.Buildings[extraPos.x, extraPos.y] = building;
        }

        Navigation.ReacalculatePath(this.Board, this.Buildings);
    }

    public void DestroyBuilding(Building building)
    {
        this.Buildings[building.GridPosition.x, building.GridPosition.y] = null;

        foreach (HexSide side in building.ExtraSize)
        {
            Vector2Int extraPos = Helpers.GetNeighborPosition(building.GridPosition, side);
            this.Buildings[extraPos.x, extraPos.y] = null;
        }

        Navigation.ReacalculatePath(this.Board, this.Buildings);

        Destroy(building.gameObject);
    }
}

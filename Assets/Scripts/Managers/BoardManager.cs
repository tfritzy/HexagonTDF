using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Dictionary<Vector2Int, Hexagon[,]> LoadedChunks;
    public Dictionary<Vector2Int, HexagonMono[,]> HexagonMonos;
    public string ActiveMapName;
    private Dictionary<Vector2Int, Building[,]> Buildings;
    public TownHall TownHall;

    void Awake()
    {
        SpawnMap();
    }

    // async void Update()
    // {
    //     Vector2Int currentChunk = Helpers.ToGridPosition(Managers.Camera.transform.position) / Constants.CHUNK_SIZE;
    // }

    private void SpawnMap()
    {
        this.LoadedChunks = new Dictionary<Vector2Int, Hexagon[,]>();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector2Int chunk = new Vector2Int(x, y);
                this.LoadedChunks[new Vector2Int(x, y)] =
                    OverworldTerrainGenerator.GenerateChunk(chunk, UnityEngine.Random.Range(0, 1000));
            }
        }

        this.Buildings = new Dictionary<Vector2Int, Building[,]>();

        // TownHall = SpawnTownHall();
        // AddBuilding(TownHall.GridPosition, TownHall);
        SpawnHexagons();
        SpawnHero();
    }

    private TownHall SpawnTownHall()
    {
        // flatten area
        var points = new List<Vector2Int> { Vector2Int.zero };
        for (int i = 0; i < 6; i++)
        {
            points.Add(Helpers.GetNeighborPosition(Vector2Int.zero, (HexSide)i));
        }

        // float averageHeight = 0;
        // List<Biome> biomesThatArentWater = new List<Biome> { LoadedChunks[Center.x, Center.y].Biome };
        // foreach (Vector2Int point in points)
        // {
        //     averageHeight += LoadedChunks[point.x, point.y].Height;

        //     if (LoadedChunks[point.x, point.y].Biome != Biome.Water)
        //     {
        //         biomesThatArentWater.Add(LoadedChunks[point.x, point.y].Biome);
        //     }
        // }

        // averageHeight /= 7;
        // int newHeight = (int)Mathf.Round(averageHeight);
        // Biome mostCommonHex = biomesThatArentWater
        //     .GroupBy(q => q)
        //     .OrderByDescending(gp => gp.Count())
        //     .First().Key;
        // foreach (Vector2Int point in points)
        // {
        //     LoadedChunks[point.x, point.y] = Prefabs.GetHexagonScript(mostCommonHex, newHeight);
        // }

        return (TownHall)InstantiateBuilding(Vector2Int.zero, BuildingType.TownHall);
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
        return Navigation.BFS(startPos, endPos, this.LoadedChunks, this.Buildings);
    }

    private void SpawnHexagons()
    {
        this.HexagonMonos = new Dictionary<Vector2Int, HexagonMono[,]>();

        foreach (Vector2Int chunkIndex in LoadedChunks.Keys)
        {
            Debug.Log("Spawning chunk, " + chunkIndex);
            Hexagon[,] chunk = LoadedChunks[chunkIndex];
            this.HexagonMonos[chunkIndex] = new HexagonMono[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];

            for (int y = 0; y < Constants.CHUNK_SIZE; y++)
            {
                for (int x = 0; x < Constants.CHUNK_SIZE; x++)
                {
                    if (chunk[x, y] == null)
                    {
                        continue;
                    }

                    BuildHexagon(chunkIndex, x, y);
                }
            }
        }

    }

    private void SpawnHero()
    {
        GameObject character = GameObject.Instantiate(Prefabs.GetCharacter(CharacterType.MainCharacter));
        character.transform.position = Helpers.ToWorldPosition(Vector2Int.zero, Vector2Int.zero + new Vector2Int(4, 4));
        character.name = "Main Character";
    }

    private bool isValidPath(List<Vector2Int> path, Vector2Int expectedStart, Vector2Int expectedEnd)
    {
        return path?.Count > 0 && path[0] == expectedStart && path.Last() == expectedEnd;
    }

    private void BuildHexagon(Vector2Int chunkIndex, int x, int y)
    {
        Helpers.GetFromChunk(this.LoadedChunks, x, y, out Hexagon point);
        Vector3 position = Helpers.ToWorldPosition(chunkIndex, x, y);
        position.y = point.Height * Constants.HEXAGON_HEIGHT;
        GameObject go = Instantiate(
            Prefabs.Hexagons[point.Biome],
            position,
            Quaternion.Euler(0, UnityEngine.Random.Range(0, 5) * 60, 0),
            this.transform);
        HexagonMono hexagonScript = go.GetComponent<HexagonMono>();
        hexagonScript.SetType(Prefabs.GetHexagonScript(point.Biome, point.Height));
        this.HexagonMonos[chunkIndex][x, y] = go.GetComponent<HexagonMono>();
        this.HexagonMonos[chunkIndex][x, y].GridPosition = new Vector2Int(x, y);
        this.HexagonMonos[chunkIndex][x, y].Height = point.Height;
        this.HexagonMonos[chunkIndex][x, y].name = point.Biome + "," + point.Height.ToString();
    }

    public HexagonMono GetHex(Vector2Int pos)
    {
        Helpers.GetFromChunk<HexagonMono>(this.HexagonMonos, pos.x, pos.y, out HexagonMono hex);
        return hex;
    }

    public Building GetBuilding(Vector2Int pos)
    {
        Helpers.GetFromChunk<Building>(this.Buildings, pos.x, pos.y, out Building building);
        return building;
    }

    public void AddBuilding(Vector2Int pos, Building building)
    {
        Helpers.SetInChunk(this.Buildings, pos.x, pos.y, building);

        foreach (HexSide side in building.ExtraSize)
        {
            Vector2Int extraPos = Helpers.GetNeighborPosition(pos, side);
            Helpers.SetInChunk(this.Buildings, extraPos.x, extraPos.y, building);
        }
    }

    public void DestroyBuilding(Building building)
    {
        Helpers.SetInChunk(this.Buildings, building.GridPosition.x, building.GridPosition.y, null);

        foreach (HexSide side in building.ExtraSize)
        {
            Vector2Int extraPos = Helpers.GetNeighborPosition(building.GridPosition, side);
            Helpers.SetInChunk(this.Buildings, extraPos.x, extraPos.y, null);
        }

        Destroy(building.gameObject);
    }

    public LinkedList<Vector2Int> GetPathBetweenPoints(Vector2Int startPos, Vector2Int endPos)
    {
        return Navigation.BFS(startPos, endPos, this.LoadedChunks, this.Buildings);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Dictionary<Vector2Int, Hexagon[,]> LoadedChunks;
    public Dictionary<Vector2Int, HexagonMono[,]> HexagonMonos;
    public string ActiveMapName;
    private Dictionary<Vector2Int, Building[,]> Buildings;
    public TownHall TownHall;
    private const int CHUNK_RENDER_DIST = 4;

    private Vector2Int renderedChunk = new Vector2Int(int.MinValue, int.MinValue);
    private Dictionary<Vector2Int, Coroutine> chunkLoadingCoroutines = new Dictionary<Vector2Int, Coroutine>();

    void Awake()
    {
        SpawnMap();
    }

    void Update()
    {
        Vector2Int currentChunk = Helpers.ToGridPosition(Managers.Camera.transform.position) / Constants.CHUNK_SIZE;
        if (currentChunk != renderedChunk)
        {
            UpdatedLoadedChunks(currentChunk);
            renderedChunk = currentChunk;
        }
    }

    private void UpdatedLoadedChunks(Vector2Int currentChunk)
    {
        HashSet<Vector2Int> chunksInRange = new HashSet<Vector2Int>();
        for (int x = currentChunk.x - CHUNK_RENDER_DIST; x < currentChunk.x + CHUNK_RENDER_DIST; x++)
        {
            for (int y = currentChunk.y - CHUNK_RENDER_DIST; y < currentChunk.y + CHUNK_RENDER_DIST; y++)
            {
                Vector2Int chunkIndex = new Vector2Int(x, y);
                chunksInRange.Add(chunkIndex);
                if (!LoadedChunks.ContainsKey(chunkIndex) && !chunkLoadingCoroutines.ContainsKey(chunkIndex))
                {
                    var coroutine = StartCoroutine(LoadChunk(chunkIndex));
                    chunkLoadingCoroutines.Add(chunkIndex, coroutine);
                }
            }
        }

        foreach (Vector2Int chunkIndex in LoadedChunks.Keys.ToList())
        {
            if (!chunksInRange.Contains(chunkIndex))
            {
                StartCoroutine(UnloadChunk(chunkIndex));
            }
        }
    }

    private IEnumerator LoadChunk(Vector2Int chunkIndex)
    {
        this.LoadedChunks[chunkIndex] = OverworldTerrainGenerator.GenerateChunk(chunkIndex, 0);
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

            yield return null;
        }

        chunkLoadingCoroutines.Remove(chunkIndex);
    }

    private IEnumerator UnloadChunk(Vector2Int chunkIndex)
    {
        HexagonMono[,] chunk = HexagonMonos[chunkIndex];
        for (int y = 0; y < Constants.CHUNK_SIZE; y++)
        {
            for (int x = 0; x < Constants.CHUNK_SIZE; x++)
            {
                Helpers.GetFromChunk<HexagonMono>(HexagonMonos, chunkIndex, x, y, out HexagonMono hex);
                if (hex != null)
                {
                    Destroy(hex.gameObject);
                }
            }

            yield return null;
        }

        HexagonMonos.Remove(chunkIndex);
        LoadedChunks.Remove(chunkIndex);
        // not sure what to do with buildings yet.
    }

    private void SpawnMap()
    {
        this.LoadedChunks = new Dictionary<Vector2Int, Hexagon[,]>();
        this.HexagonMonos = new Dictionary<Vector2Int, HexagonMono[,]>();

        // for (int x = 0; x < 10; x++)
        // {
        //     for (int y = 0; y < 10; y++)
        //     {
        //         LoadChunk(new Vector2Int(x, y));
        //     }
        // }

        this.Buildings = new Dictionary<Vector2Int, Building[,]>();

        // TownHall = SpawnTownHall();
        // AddBuilding(TownHall.GridPosition, TownHall);
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
        Helpers.GetFromChunk(this.LoadedChunks, chunkIndex, x, y, out Hexagon hex);
        Vector3 position = Helpers.ToWorldPosition(chunkIndex, x, y);
        position.y = hex.Height * Constants.HEXAGON_HEIGHT;
        GameObject go = Instantiate(
            Prefabs.Hexagons[hex.Biome],
            position,
            Quaternion.Euler(0, UnityEngine.Random.Range(0, 5) * 60, 0),
            this.transform);
        HexagonMono hexagonScript = go.GetComponent<HexagonMono>();
        hexagonScript.SetType(Prefabs.GetHexagonScript(hex.Biome, hex.Height));
        this.HexagonMonos[chunkIndex][x, y] = go.GetComponent<HexagonMono>();
        this.HexagonMonos[chunkIndex][x, y].GridPosition = new Vector2Int(x, y);
        this.HexagonMonos[chunkIndex][x, y].Height = hex.Height;
        this.HexagonMonos[chunkIndex][x, y].name = hex.Biome + "," + hex.Height.ToString();
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

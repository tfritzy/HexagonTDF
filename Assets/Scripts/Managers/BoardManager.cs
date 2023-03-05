using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public World World;
    public string ActiveMapName;
    public TownHall TownHall;
    private const int CHUNK_RENDER_DIST = 3;

    private Vector2Int renderedChunk = new Vector2Int(int.MinValue, int.MinValue);
    private Dictionary<Vector2Int, Coroutine> chunkLoadingCoroutines = new Dictionary<Vector2Int, Coroutine>();
    private int seed;
    private Dictionary<Biome, LinkedList<GameObject>> hexPool = new Dictionary<Biome, LinkedList<GameObject>>();
    private bool isWorldGenerated;

    void Awake()
    {
        this.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        StartCoroutine(SpawnMap());
    }

    void Update()
    {
        if (!isWorldGenerated)
        {
            return;
        }

        Vector2Int currentChunk = Helpers.ToGridPosition(Managers.Camera.transform.position) / Constants.CHUNK_SIZE;
        if (currentChunk != renderedChunk)
        {
            UpdatedLoadedChunks(currentChunk);
            renderedChunk = currentChunk;
        }
    }

    private void UnloadHex(Vector2Int chunkIndex, int x, int y, int z)
    {
        Hexagon destroyedHex = World.GetHex(chunkIndex, x, y, z);
        if (World.TryGetHexBody(chunkIndex, x, y, z, out HexagonMono body))
        {
            body.gameObject.SetActive(false);

            if (!hexPool.ContainsKey(destroyedHex.Biome))
            {
                hexPool[destroyedHex.Biome] = new LinkedList<GameObject>();
            }

            hexPool[destroyedHex.Biome].AddLast(body.gameObject);
        }
    }

    public void DestroyHex(int x, int y, int z)
    {
        Helpers.WorldToChunkPos(x, y, z, out Vector2Int chunkIndex, out Vector3Int subPos);
        UnloadHex(chunkIndex, subPos.x, subPos.y, z);
        World.DestroyHex(chunkIndex, subPos.x, subPos.y, z);
        foreach (Vector3Int pos in World.Chunks[chunkIndex].NeedsBody.ToList())
        {
            Hexagon hex = World.Chunks[chunkIndex].GetHex(pos.x, pos.y, pos.z);
            SpawnHex(hex, chunkIndex, pos.x, pos.y, pos.z);
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
                if (World.Chunks.ContainsKey(chunkIndex) && !chunkLoadingCoroutines.ContainsKey(chunkIndex) && !World.Chunks[chunkIndex].IsActive)
                {
                    var coroutine = StartCoroutine(LoadChunk(chunkIndex));
                    chunkLoadingCoroutines.Add(chunkIndex, coroutine);
                }
                else if (World.Chunks.ContainsKey(chunkIndex) && !World.Chunks[chunkIndex].IsActive)
                {
                    World.Chunks[chunkIndex].IsActive = true;
                }
            }
        }

        foreach (Vector2Int chunkIndex in World.Chunks.Keys.ToList())
        {
            if (!chunksInRange.Contains(chunkIndex))
            {
                UnloadChunk(chunkIndex);
            }
        }
    }

    private void GenerateChunk(Vector2Int chunkIndex)
    {
        var chunkContainer = new GameObject("Chunk " + chunkIndex);
        var terrainGenerator = new TerrainGenerator();
        terrainGenerator.GenerateChunk(chunkIndex, this.seed, chunkContainer.transform);
        World.Chunks[chunkIndex] = terrainGenerator.GeneratedChunk;
        Chunk chunk = World.Chunks[chunkIndex];
    }

    private IEnumerator LoadChunk(Vector2Int chunkIndex)
    {
        int i = 0;
        foreach (Vector3Int pos in World.Chunks[chunkIndex].NeedsBody.ToList())
        {
            Hexagon hex = World.Chunks[chunkIndex].GetHex(pos.x, pos.y, pos.z);
            bool isFromCache = SpawnHex(hex, chunkIndex, pos.x, pos.y, pos.z);
            if (isFromCache)
            {
                i += 1;
            }

            if (i % 40 == 0)
            {
                yield return null;
            }
        }

        World.Chunks[chunkIndex].IsActive = true;
        chunkLoadingCoroutines.Remove(chunkIndex);
    }

    private void UnloadChunk(Vector2Int chunkIndex)
    {
        Chunk chunk = World.Chunks[chunkIndex];
        chunk.IsActive = false;
    }

    private IEnumerator SpawnMap()
    {
        this.World = new World();

        for (int x = 0; x < Constants.WORLD_CHUNK_WIDTH; x++)
        {
            for (int y = 0; y < Constants.WORLD_CHUNK_WIDTH; y++)
            {
                GenerateChunk(new Vector2Int(x, y));
            }

            yield return null;
        }

        Debug.Log("Done generating world");

        yield return LoadChunk(new Vector2Int(10, 10));

        isWorldGenerated = true;

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

        return (TownHall)InstantiateBuilding(Vector2Int.zero, BuildingType.TownHall);
    }

    private Building InstantiateBuilding(Vector2Int pos, BuildingType type)
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
        return Navigation.BFS(startPos, endPos, World);
    }

    private void SpawnHero()
    {
        GameObject character = GameObject.Instantiate(Prefabs.GetCharacter(CharacterType.MainCharacter));
        Vector3Int spawnPos = new Vector3Int(4, 4, this.World.GetTopHexHeight(new Vector2Int(10, 10), 4, 4));
        character.transform.position = Helpers.ToWorldPosition(new Vector2Int(10, 10), spawnPos);
        character.name = "Main Character";
    }

    private bool isValidPath(List<Vector2Int> path, Vector2Int expectedStart, Vector2Int expectedEnd)
    {
        return path?.Count > 0 && path[0] == expectedStart && path.Last() == expectedEnd;
    }

    // Spawns a hex and returns true if it was retrieved from the cache, and false if instantiated.
    // The distinction is useful for knowing when the coroutine should yield.
    private bool SpawnHex(Hexagon hex, Vector2Int chunkIndex, int x, int y, int z)
    {
        if (hex == null)
        {
            return true;
        }

        GameObject go;
        if (hexPool.ContainsKey(hex.Biome) && hexPool[hex.Biome].Count > 0)
        {
            go = hexPool[hex.Biome].First.Value;
            hexPool[hex.Biome].RemoveFirst();
            go.SetActive(true);
            go.transform.SetParent(World.Chunks[chunkIndex].Container);
            AddHexGameObjectToWorld(hex, go, chunkIndex, x, y, z);
            return true;
        }
        else
        {
            go = Instantiate(
                Prefabs.Hexagons[hex.Biome],
                Vector3.zero,
                Quaternion.Euler(0, UnityEngine.Random.Range(0, 5) * 60, 0),
                World.Chunks[chunkIndex].Container.transform);
            AddHexGameObjectToWorld(hex, go, chunkIndex, x, y, z);
            return false;
        }
    }

    private void AddHexGameObjectToWorld(Hexagon hex, GameObject body, Vector2Int chunkIndex, int x, int y, int z)
    {
        Vector3 position = Helpers.ToWorldPosition(chunkIndex, x, y);
        position.y = z * Constants.HEXAGON_HEIGHT;
        body.transform.position = position;
        HexagonMono hexagonScript = body.GetComponent<HexagonMono>();
        var hexBody = body.GetComponent<HexagonMono>();
        hexBody.Setup(hex, this.seed, chunkIndex, new Vector3Int(x, y, z));
        World.SetHexBody(chunkIndex, x, y, z, hexBody);
    }

    public Building GetBuilding(Vector2Int pos)
    {
        Helpers.WorldToChunkPos(pos, out Vector2Int chunkIndex, out Vector3Int subPos);
        World.TryGetBuilding(pos.x, pos.y, World.GetTopHexHeight(chunkIndex, subPos.x, subPos.y) + 1, out Building building);
        return building;
    }

    public void SetBuilding(Vector2Int pos, Building building)
    {
        Building currentBuilding = GetBuilding(pos);
        if (building != null && currentBuilding != null && !currentBuilding.IsPreview && currentBuilding != building)
        {
            Debug.LogError($"Tried to place a building at {pos} but it's already occupied by {currentBuilding?.name}");
        }

        Helpers.WorldToChunkPos(pos, out Vector2Int chunkIndex, out Vector3Int subPos);
        World.SetBuilding(
            chunkIndex,
            subPos.x,
            subPos.y,
            World.GetTopHexHeight(chunkIndex, subPos.x, subPos.y) + 1,
            building);

        if (building == null && currentBuilding != null)
        {
            foreach (HexSide side in currentBuilding.ExtraSize)
            {
                Vector2Int extraPos = Helpers.GetNeighborPosition(pos, side);
                Helpers.WorldToChunkPos(extraPos, out chunkIndex, out subPos);
                World.SetBuilding(
                    chunkIndex,
                    extraPos.x,
                    extraPos.y,
                    World.GetTopHexHeight(chunkIndex, subPos.x, subPos.y) + 1,
                    building);
            }
        }
        else if (building != null)
        {
            foreach (HexSide side in building.ExtraSize)
            {
                Vector2Int extraPos = Helpers.GetNeighborPosition(pos, side);
                Helpers.WorldToChunkPos(extraPos, out chunkIndex, out subPos);

                currentBuilding = GetBuilding(extraPos);
                if (currentBuilding != null && !currentBuilding.IsPreview && currentBuilding != building)
                {
                    Debug.LogError($"Tried to place a building at {extraPos} but it's already occupied");
                }

                World.SetBuilding(
                    chunkIndex,
                    extraPos.x,
                    extraPos.y,
                    World.GetTopHexHeight(chunkIndex, subPos.x, subPos.y) + 1,
                    building);
            }
        }
    }

    public LinkedList<Vector2Int> GetPathBetweenPoints(Vector2Int startPos, Vector2Int endPos)
    {
        return Navigation.BFS(startPos, endPos, this.World);
    }

    public Building BuildBuilding(BuildingType buildingType, Vector2Int gridPosition)
    {
        Building currentBulding = GetBuilding(gridPosition);

        // You can build on top of a preview, but not a real building.
        if (currentBulding != null && !currentBulding.IsPreview)
        {
            if (currentBulding.IsPreview)
            {
                DestroyBuilding(currentBulding);
            }
            else
            {
                return null;
            }
        }

        Building newBuilding = InstantiateBuilding(gridPosition, buildingType);
        SetBuilding(gridPosition, newBuilding);
        return newBuilding;
    }

    public void DestroyBuilding(Building building)
    {
        if (GetBuilding(building.GridPosition) == building)
        {
            SetBuilding(building.GridPosition, null);
        }

        building.OnDestroy();
        Destroy(building.gameObject);
    }
}
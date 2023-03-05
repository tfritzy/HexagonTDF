using System.Collections.Generic;
using UnityEngine;

public class World
{
    public Dictionary<Vector2Int, Chunk> Chunks = new Dictionary<Vector2Int, Chunk>();

    public void DestroyHex(Vector2Int chunkIndex, int x, int y, int z)
    {
        Chunks[chunkIndex].DestroyHex(x, y, z);
    }

    public bool TryGetBuilding(int x, int y, int z, out Building building)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        return TryGetBuilding(chunkIndex, x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, z, out building);
    }

    public HashSet<int> GetUncoveredHexOfColumn(Vector2Int chunkIndex, int x, int y)
    {
        return Chunks[chunkIndex].GetUncoveredOfColumn(x, y);
    }

    public int GetTopHexHeight(Vector2Int chunkIndex, int x, int y)
    {
        if (!Chunks.ContainsKey(chunkIndex))
        {
            return 0;
        }

        return Chunks[chunkIndex].GetTopHex(x, y);
    }

    public Hexagon GetTopHex(Vector2Int chunkIndex, int x, int y)
    {
        int height = Chunks[chunkIndex].GetTopHex(x, y);
        return Chunks[chunkIndex].GetHex(x, y, height);
    }

    public HexagonMono GetTopHexBody(Vector2Int chunkIndex, int x, int y)
    {
        int height = Chunks[chunkIndex].GetTopHex(x, y);
        return Chunks[chunkIndex].GetBody(x, y, height);
    }

    public Hexagon GetHex(Vector2Int chunkIndex, int x, int y, int z)
    {
        return Chunks[chunkIndex].GetHex(x, y, z);
    }

    public bool TryGetBuilding(Vector2Int chunkIndex, int x, int y, int z, out Building building)
    {
        if (!Chunks.ContainsKey(chunkIndex))
        {
            building = null;
            return false;
        }

        return Chunks[chunkIndex].TryGetBuilding(x, y, z, out building);
    }

    public void SetBuilding(Vector2Int chunkIndex, int x, int y, int z, Building value)
    {
        if (!Chunks.ContainsKey(chunkIndex))
        {
            Debug.LogWarning("Tried to set value in unloaded chunk");
            return;
        }

        Chunks[chunkIndex].SetBuilding(x, y, z, value);
    }

    public bool TryGetHexBody(Vector2Int chunkIndex, int x, int y, int z, out HexagonMono hex)
    {
        if (!Chunks.ContainsKey(chunkIndex))
        {
            hex = null;
            return false;
        }

        hex = Chunks[chunkIndex].GetBody(x, y, z);
        return hex != null;
    }

    public void SetHexBody(Vector2Int chunkIndex, int x, int y, int z, HexagonMono value)
    {
        if (!Chunks.ContainsKey(chunkIndex))
        {
            Debug.LogWarning("Tried to set value in unloaded chunk");
            return;
        }

        Chunks[chunkIndex].SetBody(x, y, z, value);
    }
}
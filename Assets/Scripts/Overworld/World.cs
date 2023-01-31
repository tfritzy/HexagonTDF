using System.Collections.Generic;
using UnityEngine;

public class World
{
    public Dictionary<Vector2Int, Chunk> Chunks = new Dictionary<Vector2Int, Chunk>();

    public Hexagon

    public bool TryGetBuilding(int x, int y, int z, out Building building)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        return TryGetBuilding(chunkIndex, x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, z, out building);
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

    public void SetBuilding(int x, int y, int z, Building value)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        SetBuilding(chunkIndex, x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, z, value);
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

    public bool TryGetHexBody(int x, int y, int z, out HexagonMono hex)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        return TryGetHexBody(chunkIndex, x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, z, out hex);
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

    public void SetHexBody(int x, int y, int z, HexagonMono value)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        SetHexBody(chunkIndex, x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, z, value);
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
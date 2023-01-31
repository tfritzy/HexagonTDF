using System.Collections.Generic;
using UnityEngine;

public class World
{
    public Dictionary<Vector2Int, Chunk> Chunks = new Dictionary<Vector2Int, Chunk>();

    public bool TryGetBuilding(int x, int y, int z, out Building building)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        return TryGetBuilding(chunkIndex, x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, z, out building);
    }

    public LinkedList<int> GetUncoveredHexOfColumn(int x, int y)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        return Chunks[chunkIndex].GetUncoveredOfColumn(x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE);
    }

    public int GetTopHexHeight(int x, int y)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        return Chunks[chunkIndex].GetTopHex(x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE);
    }

    public Hexagon GetTopHex(int x, int y)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        int height = Chunks[chunkIndex].GetTopHex(x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE);
        return Chunks[chunkIndex].GetHex(x, y, height);
    }

    public Hexagon GetHex(int x, int y, int z)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        return Chunks[chunkIndex].GetHex(x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, z);
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

    public HexagonMono GetTopHexBody(int x, int y)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        int height = Chunks[chunkIndex].GetTopHex(x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE);
        return Chunks[chunkIndex].GetBody(x, y, height);
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
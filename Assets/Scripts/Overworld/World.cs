using System.Collections.Generic;
using UnityEngine;

public class World
{
    public Dictionary<Vector2Int, Chunk> Chunks = new Dictionary<Vector2Int, Chunk>();

    public bool TryGetHex(int x, int y, out Hexagon hex)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        return TryGetHex(chunkIndex, x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, out hex);
    }

    public bool TryGetHex(Vector2Int chunkIndex, int x, int y, out Hexagon hex)
    {
        if (!Chunks.ContainsKey(chunkIndex))
        {
            hex = null;
            return false;
        }

        hex = Chunks[chunkIndex].Hexes[x, y];
        return true;
    }

    public bool TryGetBuilding(int x, int y, out Building building)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        return TryGetBuilding(chunkIndex, x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, out building);
    }

    public bool TryGetBuilding(Vector2Int chunkIndex, int x, int y, out Building building)
    {
        if (!Chunks.ContainsKey(chunkIndex))
        {
            building = null;
            return false;
        }

        building = Chunks[chunkIndex].Buildings[x, y];
        return true;
    }

    public void SetBuilding(int x, int y, Building value)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        SetBuilding(chunkIndex, x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, value);
    }

    public void SetBuilding(Vector2Int chunkIndex, int x, int y, Building value)
    {
        if (!Chunks.ContainsKey(chunkIndex))
        {
            Debug.LogWarning("Tried to set value in unloaded chunk");
            return;
        }

        Chunks[chunkIndex].Buildings[x, y] = value;
    }

    public bool TryGetHexBody(int x, int y, out HexagonMono hex)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        return TryGetHexBody(chunkIndex, x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, out hex);
    }

    public bool TryGetHexBody(Vector2Int chunkIndex, int x, int y, out HexagonMono hex)
    {
        if (!Chunks.ContainsKey(chunkIndex))
        {
            hex = null;
            return false;
        }

        hex = Chunks[chunkIndex].HexBodies[x, y];
        return true;
    }

    public void SetHexBody(int x, int y, HexagonMono value)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        SetHexBody(chunkIndex, x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE, value);
    }

    public void SetHexBody(Vector2Int chunkIndex, int x, int y, HexagonMono value)
    {
        if (!Chunks.ContainsKey(chunkIndex))
        {
            Debug.LogWarning("Tried to set value in unloaded chunk");
            return;
        }

        Chunks[chunkIndex].HexBodies[x, y] = value;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private Hexagon[,,] Hexes = new Hexagon[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.HEIGHT_LEVELS];
    private HexagonMono[,,] HexBodies = new HexagonMono[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.HEIGHT_LEVELS];
    private Building[,,] Buildings = new Building[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.HEIGHT_LEVELS];

    // The visible hexes on each column in the chunk.
    private Stack<int>[,] TopLayerHeights = new Stack<int>[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];

    public Chunk(Hexagon[,,] hexes)
    {
        this.Hexes = hexes;
    }

    public bool TryGetHex(Vector2Int chunkIndex, int x, int y, int z, out Hexagon hex)
    {
        if (!Helpers.IsInBounds(x, y, Constants.CHUNK_DIMENSIONS))
        {
            hex = null;
            return false;
        }

        hex = Hexes[x, y, z];
        return true;
    }

    public HexagonMono GetBody(int x, int y, int z)
    {
        // Allow passing in absolute position.
        if (x >= Constants.CHUNK_SIZE || y >= Constants.CHUNK_SIZE)
        {
            x %= Constants.CHUNK_SIZE;
            y %= Constants.CHUNK_SIZE;
        }

        return HexBodies[x, y, z];
    }

    public HexagonMono SetBody(int x, int y, int z, HexagonMono value)
    {
        // Allow passing in absolute position.
        if (x >= Constants.CHUNK_SIZE || y >= Constants.CHUNK_SIZE)
        {
            x %= Constants.CHUNK_SIZE;
            y %= Constants.CHUNK_SIZE;
        }

        return HexBodies[x, y, z] = value;
    }

    public bool TryGetBuilding(int x, int y, int z, out Building building)
    {
        if (!Helpers.IsInBounds(x, y, Constants.CHUNK_DIMENSIONS))
        {
            building = null;
            return false;
        }

        building = Buildings[x, y, z];
        return true;
    }

    public void SetBuilding(int x, int y, int z, Building value)
    {
        Buildings[x, y, z] = value;
    }
}
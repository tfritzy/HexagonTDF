using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk
{
    public HashSet<Vector3Int> NeedsBody = new HashSet<Vector3Int>();
    public Transform Container { get; private set; }

    private Hexagon[,,] Hexes = new Hexagon[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.MAX_HEIGHT];
    private HexagonMono[,,] HexBodies = new HexagonMono[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.MAX_HEIGHT];
    private Building[,,] Buildings = new Building[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.MAX_HEIGHT];

    // The visible hexes on each column in the chunk.
    private HashSet<int>[,] UncoveredBlocks = new HashSet<int>[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];
    private Vector2Int ChunkIndex;

    public Chunk(Vector2Int chunkIndex, Hexagon[,,] hexes, int[,] tops, Transform container)
    {
        this.ChunkIndex = chunkIndex;
        this.Hexes = hexes;
        this.Container = container;

        for (int y = 0; y < Constants.CHUNK_SIZE; y++)
        {
            for (int x = 0; x < Constants.CHUNK_SIZE; x++)
            {
                UncoveredBlocks[x, y] = new HashSet<int>();
                UncoveredBlocks[x, y].Add(tops[x, y]);
                NeedsBody.Add(new Vector3Int(x, y, tops[x, y]));

                int lowestNeighbor = GetLowestNeighbor(tops, x, y);
                int iHeight = tops[x, y];
                while (iHeight > 0 && iHeight > lowestNeighbor)
                {
                    UncoveredBlocks[x, y].Add(iHeight);
                    NeedsBody.Add(new Vector3Int(x, y, iHeight));
                    iHeight -= 1;
                }
            }
        }
    }

    private int GetLowestNeighbor(int[,] tops, int x, int y)
    {
        int lowest = tops[x, y];
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighbor = Helpers.GetNeighborPosition(x, y, (HexSide)i);
            if (Helpers.IsInBounds(neighbor, Constants.CHUNK_DIMENSIONS))
            {
                if (tops[neighbor.x, neighbor.y] < lowest)
                {
                    lowest = tops[neighbor.x, neighbor.y];
                }
            }
        }

        return lowest;
    }

    public void DestroyHex(int x, int y, int z)
    {
        if (!Helpers.IsInBounds(x, y, z, Constants.CHUNK_DIMENSIONS))
        {
            return;
        }

        Hexes[x, y, z] = null;

        this.HexBodies[x, y, z] = null;
        this.UncoveredBlocks[x, y].Remove(z);
        UncoverNeighbors(x, y, z);
    }

    private void UncoverNeighbors(int x, int y, int z)
    {
        MaybeSetUncovered(x, y, z + 1);
        MaybeSetUncovered(x, y, z - 1);

        for (int i = 0; i < 6; i++)
        {
            var neighbor = Helpers.GetNeighborPosition(x, y, (HexSide)i);
            if (Helpers.IsInBounds(neighbor, Constants.CHUNK_DIMENSIONS))
            {
                MaybeSetUncovered(neighbor.x, neighbor.y, z);
            }
            else
            {
                // TODO: inform neighboring chunks of uncovered status
            }
        }
    }

    private void MaybeSetUncovered(int x, int y, int z)
    {
        if (!Helpers.IsInBounds(x, y, z, Constants.CHUNK_DIMENSIONS))
        {
            return;
        }

        if (GetHex(x, y, z) != null)
        {
            if (UncoveredBlocks[x, y] == null)
            {
                UncoveredBlocks[x, y] = new HashSet<int>();
            }

            UncoveredBlocks[x, y].Add(z);
            if (HexBodies[x, y, z] == null)
            {
                NeedsBody.Add(new Vector3Int(x, y, z));
            }
        }
    }

    public HashSet<int> GetUncoveredOfColumn(int x, int y)
    {
        return UncoveredBlocks[x, y];
    }

    public int GetTopHex(int x, int y)
    {
        return UncoveredBlocks[x, y].Max();
    }

    public Hexagon GetHex(int x, int y, int z)
    {
        return Hexes[x, y, z];
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


    public void SetBody(int x, int y, int z, HexagonMono value)
    {
        NeedsBody.Remove(new Vector3Int(x, y, z));
        HexBodies[x, y, z] = value;
    }

    public void SetHex(int x, int y, int z, Hexagon value)
    {
        // Allow passing in absolute position.
        if (x >= Constants.CHUNK_SIZE || y >= Constants.CHUNK_SIZE)
        {
            x %= Constants.CHUNK_SIZE;
            y %= Constants.CHUNK_SIZE;
        }

        Hexes[x, y, z] = value;
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
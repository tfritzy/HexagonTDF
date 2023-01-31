using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk
{
    private Hexagon[,,] Hexes = new Hexagon[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.MAX_HEIGHT];
    private HexagonMono[,,] HexBodies = new HexagonMono[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.MAX_HEIGHT];
    private Building[,,] Buildings = new Building[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.MAX_HEIGHT];

    // The visible hexes on each column in the chunk.
    private HashSet<int>[,] UncoveredBlocks = new HashSet<int>[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];
    private Vector2Int ChunkIndex;

    public Chunk(Vector2Int chunkIndex, Hexagon[,,] hexes)
    {
        this.ChunkIndex = chunkIndex;
        this.Hexes = hexes;
        CalculateUncoveredHex();
    }

    private void CalculateUncoveredHex()
    {
        this.UncoveredBlocks = new HashSet<int>[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];

        for (int x = 0; x < Constants.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Constants.CHUNK_SIZE; y++)
            {
                for (int z = Constants.MAX_HEIGHT - 1; z >= 0; z--)
                {
                    // Mark every hex touching air or transparent block as uncovered.
                    if (this.Hexes[x, y, z] == null || this.Hexes[x, y, z].IsTransparent)
                    {
                        if (UncoveredBlocks[x, y] == null)
                        {
                            UncoveredBlocks[x, y] = new HashSet<int>();
                        }

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
                    else
                    {
                        // Once we hit an opaque block we can stop. Not doing caves.
                        break;
                    }
                }
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
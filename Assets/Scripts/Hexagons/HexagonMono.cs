using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonMono : MonoBehaviour, Interactable
{
    public Biome Biome { get { return Hexagon.Biome; } }
    public bool IsBuildable { get { return Hexagon.IsBuildable && !HasObstacle; } }
    public bool IsWalkable { get { return Hexagon.IsWalkable && !HasObstacle; } }
    public Vector2Int GridPosition { get; private set; }
    public int Height { get; private set; }
    public Vector3Int ChunkPos { get; private set; }
    public Vector2Int Chunk { get; private set; }
    public Hexagon Hexagon { get; private set; }
    protected List<MeshRenderer> meshRenderers;
    private MeshRenderer hexMesh;
    public bool HasObstacle { get; private set; }

    public virtual void Setup(Hexagon hex, int seed, Vector2Int chunkPos, Vector3Int chunkSubPos)
    {
        this.Hexagon = hex;
        Helpers.ChunkToWorldPos(chunkPos, chunkSubPos, out Vector3Int gridPosition);
        this.GridPosition = (Vector2Int)gridPosition;
        this.Height = gridPosition.z;
        this.ChunkPos = chunkSubPos;
        this.Chunk = chunkPos;
        this.hexMesh = transform.Find("hex")?.GetComponent<MeshRenderer>();
        FindMeshRenderers();
        InitObstacle(seed);
        InitDecorations();
    }

    public void Interact()
    {
        Managers.Board.DestroyHex(this.GridPosition.x, this.GridPosition.y, this.Height);
    }

    private void InitObstacle(int seed)
    {
        RollObstacle(seed);
        if (this.HasObstacle)
        {
            GameObject body = this.Hexagon.GetObstacleBody();
            GameObject.Instantiate(body, this.transform.position, body.transform.rotation, this.transform);
        }
    }

    private void RollObstacle(int seed)
    {
        System.Random random = new System.Random(seed + GridPosition.GetHashCode());
        if (Hexagon.ObstacleChance > 0 && random.NextDouble() < Hexagon.ObstacleChance)
        {
            HasObstacle = true;
        }
    }

    private void InitDecorations()
    {
        for (int i = 0; i < this.Hexagon.NumDecorations; i++)
        {
            GameObject body = this.Hexagon.GetDecorationBody();
            GameObject.Instantiate(body, this.transform.position, Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0), this.transform);
        }
    }

    private void FindMeshRenderers()
    {
        this.meshRenderers = new List<MeshRenderer>();
        foreach (MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderers.Add(renderer);
        }
    }

    // This shouldn't be useful, but I don't have the power to delete it

    // // Injects side data into the hex to be used for ambient occlusion
    // public void SetSideData()
    // {
    //     List<Vector2Int> vertexSides = new List<Vector2Int>
    //     {
    //         new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.SouthEast, HexSide.South)),
    //         new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.NorthWest, HexSide.SouthWest)),
    //         new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.SouthWest, HexSide.South)),
    //         new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.North, HexSide.NorthEast)),
    //         new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.North, HexSide.NorthWest)),
    //         new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.NorthEast, HexSide.SouthEast)),
    //         new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.NorthEast),
    //                        GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.SouthEast)),
    //         new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.South),
    //                        GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.SouthEast)),
    //         new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.North),
    //                        GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.NorthEast)),
    //         new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.NorthWest),
    //                        GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.SouthWest)),
    //         new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.NorthWest),
    //                        GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.North)),
    //         new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.SouthWest),
    //                        GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.South)),
    //     };

    //     int key = 0;
    //     for (int i = 0; i < vertexSides.Count; i++)
    //     {
    //         if (vertexSides[i].x == 1)
    //         {
    //             key = key | 1 << (i * 2);
    //         }

    //         if (vertexSides[i].y == 1)
    //         {
    //             key = key | 1 << (i * 2 + 1);
    //         }
    //     }

    //     if (aoMeshCache == null)
    //     {
    //         aoMeshCache = new Dictionary<int, Mesh>();
    //     }

    //     MeshFilter meshFilter = this.transform.Find("hex").GetComponent<MeshFilter>();
    //     if (!aoMeshCache.ContainsKey(key))
    //     {
    //         Mesh mesh = meshFilter.mesh;
    //         mesh.SetUVs(1, new List<Vector2>()
    //         {
    //             vertexSides[0], // (0.50, -0.51, -0.87)
    //             vertexSides[1], // (-1.00, -0.51, 0.00)
    //             vertexSides[2], // (-0.50, -0.51, -0.87)
    //             vertexSides[3], // (0.50, -0.51, 0.87)
    //             vertexSides[4], // (-0.50, -0.51, 0.87)
    //             vertexSides[5], // (1.00, -0.51, 0.00)
    //             new Vector2(0, 0), // (0.75, -0.25, -0.43)
    //             vertexSides[6], // (1.00, 0.01, 0.00)
    //             vertexSides[5], // (1.00, -0.51, 0.00)
    //             vertexSides[0], // (0.50, -0.51, -0.87)
    //             vertexSides[7], // (0.50, 0.01, -0.87)
    //             vertexSides[6], // (1.00, 0.01, 0.00)
    //             vertexSides[8], // (0.50, 0.01, 0.87)
    //             new Vector2(0, 0), // (0.75, -0.25, 0.43)
    //             vertexSides[5], // (1.00, -0.51, 0.00)
    //             vertexSides[3], // (0.50, -0.51, 0.87)
    //             new Vector2(0, 0), // (-0.75, -0.25, 0.43)
    //             vertexSides[9], // (-1.00, 0.01, 0.00)
    //             vertexSides[1], // (-1.00, -0.51, 0.00)
    //             vertexSides[4], // (-0.50, -0.51, 0.87)
    //             vertexSides[10], // (-0.50, 0.01, 0.87)
    //             vertexSides[1], // (-1.00, -0.51, 0.00)
    //             vertexSides[9], // (-1.00, 0.01, 0.00)
    //             new Vector2(0, 0), // (-0.75, -0.25, -0.43)
    //             vertexSides[2], // (-0.50, -0.51, -0.87)
    //             vertexSides[11], // (-0.50, 0.01, -0.87)
    //             new Vector2(0, 0), // (0.00, -0.25, -0.87)
    //             vertexSides[7], // (0.50, 0.01, -0.87)
    //             vertexSides[0], // (0.50, -0.51, -0.87)
    //             vertexSides[2], // (-0.50, -0.51, -0.87)
    //             vertexSides[11], // (-0.50, 0.01, -0.87)
    //             vertexSides[8], // (0.50, 0.01, 0.87)
    //             vertexSides[10], // (-0.50, 0.01, 0.87)
    //             new Vector2(0, 0), // (0.00, -0.25, 0.87)
    //             vertexSides[3], // (0.50, -0.51, 0.87)
    //             vertexSides[4], // (-0.50, -0.51, 0.87)
    //             vertexSides[7], // (0.50, 0.01, -0.87)
    //             vertexSides[11], // (-0.50, 0.01, -0.87)
    //             new Vector2(0, 0), // (0.00, 0.01, 0.00)
    //             vertexSides[6], // (1.00, 0.01, 0.00)
    //             vertexSides[9], // (-1.00, 0.01, 0.00)
    //             vertexSides[8], // (0.50, 0.01, 0.87)
    //             vertexSides[10], // (-0.50, 0.01, 0.87)
    //         });
    //         aoMeshCache[key] = mesh;
    //     }
    //     else
    //     {
    //         meshFilter.mesh = aoMeshCache[key];
    //     }

    // }

    // private int GetNeighborOpacity(int height, int x, int y, params HexSide[] sides)
    // {
    //     foreach (HexSide side in sides)
    //     {
    //         var n = Helpers.GetNeighborPosition(x, y, side);
    //         if (!Helpers.IsInBounds(n, Managers.Board.Dimensions))
    //         {
    //             continue;
    //         }

    //         Hexagon point = Managers.Board.LoadedChunks[n.x, n.y];
    //         if (point.Height > height)
    //         {
    //             return 1;
    //         }
    //     }

    //     return 0;
    // }
}

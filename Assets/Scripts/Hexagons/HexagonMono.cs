using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonMono : MonoBehaviour, Interactable
{
    public Biome Biome { get { return Hexagon.Biome; } }
    public bool IsBuildable { get { return Hexagon.IsBuildable; } }
    public bool IsWalkable { get { return Hexagon.IsWalkable; } }
    public Vector2Int GridPosition;
    public int Height;
    public Hexagon Hexagon;

    protected int colorVaryIndex;
    protected List<MeshRenderer> meshRenderers;

    private MeshRenderer hexMesh;
    private static Dictionary<Biome, Dictionary<int, Material>> baseMaterialCache;
    private static Dictionary<Biome, Material> outlineMaterialCache;
    private static Dictionary<int, Mesh> aoMeshCache;

    public void SetType(Hexagon hexagon)
    {
        this.Hexagon = hexagon;
    }

    void Start()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        this.hexMesh = transform.Find("hex")?.GetComponent<MeshRenderer>();
        FindMeshRenderers();
        SetHexBodyColor();
        InitObstacle();
    }

    public void Interact()
    {
        Debug.Log("Clicked on hex at pos " + GridPosition);
    }

    public void SetMaterial(Material material)
    {
        this.hexMesh.material = material;
    }

    public void InitObstacle()
    {
        this.Hexagon.RollObstacle();
        if (this.Hexagon.HasObstacle)
        {
            GameObject body = this.Hexagon.GetObstacleBody();
            GameObject.Instantiate(body, this.transform.position, body.transform.rotation);
        }
    }

    public void ResetMaterial()
    {
        SetHexBodyColor();
    }

    private void FindMeshRenderers()
    {
        this.meshRenderers = new List<MeshRenderer>();
        foreach (MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderers.Add(renderer);
        }
    }

    protected virtual void SetHexBodyColor()
    {
        if (this.hexMesh == null)
        {
            return;
        }

        if (baseMaterialCache == null)
        {
            baseMaterialCache = new Dictionary<Biome, Dictionary<int, Material>>();
        }

        if (!baseMaterialCache.ContainsKey(this.Hexagon.Biome))
        {
            baseMaterialCache[this.Hexagon.Biome] = new Dictionary<int, Material>();
        }

        System.Random random = new System.Random(GridPosition.x * 7 + GridPosition.y * 31);
        this.colorVaryIndex = random.Next(0, 3);

        if (!baseMaterialCache[this.Hexagon.Biome].ContainsKey(this.colorVaryIndex))
        {
            Color ColorAfterVariance = ColorExtensions.VaryBy(this.Hexagon.BaseColor, this.colorVaryIndex * this.Hexagon.MaxColorVariance);
            Material newBase = new Material(Prefabs.GetMaterial(MaterialType.Base));
            newBase.color = ColorAfterVariance;
            baseMaterialCache[this.Hexagon.Biome][this.colorVaryIndex] = newBase;
        }

        if (outlineMaterialCache == null)
        {
            outlineMaterialCache = new Dictionary<Biome, Material>();
        }

        this.hexMesh.material = baseMaterialCache[this.Hexagon.Biome][this.colorVaryIndex];
    }


    // Injects side data into the hex to be used for ambient occlusion
    public void SetSideData()
    {
        List<Vector2Int> vertexSides = new List<Vector2Int>
        {
            new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.SouthEast, HexSide.South)),
            new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.NorthWest, HexSide.SouthWest)),
            new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.SouthWest, HexSide.South)),
            new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.North, HexSide.NorthEast)),
            new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.North, HexSide.NorthWest)),
            new Vector2Int(1, GetNeighborOpacity(this.Height - 1, this.GridPosition.x, this.GridPosition.y, HexSide.NorthEast, HexSide.SouthEast)),
            new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.NorthEast),
                           GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.SouthEast)),
            new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.South),
                           GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.SouthEast)),
            new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.North),
                           GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.NorthEast)),
            new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.NorthWest),
                           GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.SouthWest)),
            new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.NorthWest),
                           GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.North)),
            new Vector2Int(GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.SouthWest),
                           GetNeighborOpacity(this.Height, this.GridPosition.x, this.GridPosition.y, HexSide.South)),
        };

        int key = 0;
        for (int i = 0; i < vertexSides.Count; i++)
        {
            if (vertexSides[i].x == 1)
            {
                key = key | 1 << (i * 2);
            }

            if (vertexSides[i].y == 1)
            {
                key = key | 1 << (i * 2 + 1);
            }
        }

        if (aoMeshCache == null)
        {
            aoMeshCache = new Dictionary<int, Mesh>();
        }

        MeshFilter meshFilter = this.transform.Find("hex").GetComponent<MeshFilter>();
        if (!aoMeshCache.ContainsKey(key))
        {
            Mesh mesh = meshFilter.mesh;
            mesh.SetUVs(1, new List<Vector2>()
            {
                vertexSides[0], // (0.50, -0.51, -0.87)
                vertexSides[1], // (-1.00, -0.51, 0.00)
                vertexSides[2], // (-0.50, -0.51, -0.87)
                vertexSides[3], // (0.50, -0.51, 0.87)
                vertexSides[4], // (-0.50, -0.51, 0.87)
                vertexSides[5], // (1.00, -0.51, 0.00)
                new Vector2(0, 0), // (0.75, -0.25, -0.43)
                vertexSides[6], // (1.00, 0.01, 0.00)
                vertexSides[5], // (1.00, -0.51, 0.00)
                vertexSides[0], // (0.50, -0.51, -0.87)
                vertexSides[7], // (0.50, 0.01, -0.87)
                vertexSides[6], // (1.00, 0.01, 0.00)
                vertexSides[8], // (0.50, 0.01, 0.87)
                new Vector2(0, 0), // (0.75, -0.25, 0.43)
                vertexSides[5], // (1.00, -0.51, 0.00)
                vertexSides[3], // (0.50, -0.51, 0.87)
                new Vector2(0, 0), // (-0.75, -0.25, 0.43)
                vertexSides[9], // (-1.00, 0.01, 0.00)
                vertexSides[1], // (-1.00, -0.51, 0.00)
                vertexSides[4], // (-0.50, -0.51, 0.87)
                vertexSides[10], // (-0.50, 0.01, 0.87)
                vertexSides[1], // (-1.00, -0.51, 0.00)
                vertexSides[9], // (-1.00, 0.01, 0.00)
                new Vector2(0, 0), // (-0.75, -0.25, -0.43)
                vertexSides[2], // (-0.50, -0.51, -0.87)
                vertexSides[11], // (-0.50, 0.01, -0.87)
                new Vector2(0, 0), // (0.00, -0.25, -0.87)
                vertexSides[7], // (0.50, 0.01, -0.87)
                vertexSides[0], // (0.50, -0.51, -0.87)
                vertexSides[2], // (-0.50, -0.51, -0.87)
                vertexSides[11], // (-0.50, 0.01, -0.87)
                vertexSides[8], // (0.50, 0.01, 0.87)
                vertexSides[10], // (-0.50, 0.01, 0.87)
                new Vector2(0, 0), // (0.00, -0.25, 0.87)
                vertexSides[3], // (0.50, -0.51, 0.87)
                vertexSides[4], // (-0.50, -0.51, 0.87)
                vertexSides[7], // (0.50, 0.01, -0.87)
                vertexSides[11], // (-0.50, 0.01, -0.87)
                new Vector2(0, 0), // (0.00, 0.01, 0.00)
                vertexSides[6], // (1.00, 0.01, 0.00)
                vertexSides[9], // (-1.00, 0.01, 0.00)
                vertexSides[8], // (0.50, 0.01, 0.87)
                vertexSides[10], // (-0.50, 0.01, 0.87)
            });
            aoMeshCache[key] = mesh;
        }
        else
        {
            meshFilter.mesh = aoMeshCache[key];
        }

    }

    private int GetNeighborOpacity(int height, int x, int y, params HexSide[] sides)
    {
        foreach (HexSide side in sides)
        {
            var n = Helpers.GetNeighborPosition(x, y, side);
            if (!Helpers.IsInBounds(n, Managers.Board.Dimensions))
            {
                continue;
            }

            Hexagon point = Managers.Board.Board[n.x, n.y];
            if (point.Height > height)
            {
                return 1;
            }
        }

        return 0;
    }
}

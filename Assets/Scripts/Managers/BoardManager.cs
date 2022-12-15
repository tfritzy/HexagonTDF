﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public HexagonMono[,] Hexagons;
    public string ActiveMapName;
    public bool RegenerateMap;
    private Building[,] Buildings;
    private RectInt Dimensions;
    private const float HEX_HEIGHT = .5f;
    private OverworldSegment CurrentSegment;
    private const int WaterHeight = 2;

    void Awake()
    {
        SpawnMap();
    }

    void Update()
    {
        if (RegenerateMap)
        {
            CleanupMap();
            SpawnMap();
            RegenerateMap = false;
        }
    }

    private void SpawnMap()
    {
        if (GameState.SelectedSegment == null)
        {
            CurrentSegment = OverworldTerrainGenerator.GenerateSingleSegment(25, 25, UnityEngine.Random.Range(0, 10));
        }
        else
        {
            CurrentSegment = GameState.SelectedSegment;
        }

        SpawnHexagons();
    }

    private void CleanupMap()
    {
        foreach (HexagonMono hexagon in this.Hexagons)
        {
            Destroy(hexagon?.gameObject);
        }
    }

    private void SpawnHexagons()
    {
        this.Hexagons = new HexagonMono[CurrentSegment.Width, CurrentSegment.Height];
        this.Buildings = new Building[CurrentSegment.Width, CurrentSegment.Height];
        this.Dimensions = new RectInt(0, 0, CurrentSegment.Width, CurrentSegment.Height);

        for (int y = 0; y < CurrentSegment.Height; y++)
        {
            for (int x = 0; x < CurrentSegment.Width; x++)
            {
                if (CurrentSegment.GetPoint(x, y) == null)
                {
                    continue;
                }

                BuildHexagon(x, y, CurrentSegment.Index);
            }
        }
    }

    private bool isValidPath(List<Vector2Int> path, Vector2Int expectedStart, Vector2Int expectedEnd)
    {
        return path?.Count > 0 && path[0] == expectedStart && path.Last() == expectedEnd;
    }

    private void BuildHexagon(int x, int y, int segmentIndex)
    {
        OverworldMapPoint point = CurrentSegment.GetPoint(x, y);
        Vector3 position = Helpers.ToWorldPosition(x, y);
        position.y = point.Height * HEX_HEIGHT;
        GameObject go = Instantiate(Prefabs.Hexagons[point.Biome], position, new Quaternion(), this.transform);
        HexagonMono hexagonScript = go.GetComponent<HexagonMono>();
        hexagonScript.SetType(Prefabs.GetHexagonScript(point.Biome));
        this.Hexagons[x, y] = go.GetComponent<HexagonMono>();
        this.Hexagons[x, y].GridPosition = new Vector2Int(x, y);
        this.Hexagons[x, y].Height = point.Height;
        this.Hexagons[x, y].name = point.Height.ToString();
        this.SetSideData(go, point.Height, x, y);

        if (point.Height > WaterHeight)
        {
            this.Hexagons[x, y].MaybeSpawnObstacle(segmentIndex);
        }
    }

    // Injects side data into the hex to be used for ambient occlusion
    public void SetSideData(GameObject hex, int height, int x, int y)
    {
        Mesh mesh = hex.transform.Find("hex").GetComponent<MeshFilter>().mesh;

        List<Vector2> vertexSides = new List<Vector2>
        {
            /* new Vector2(0, 0),  //  */new Vector2(1, GetNeighborOpacity(height - 1, x, y, HexSide.SouthEast, HexSide.South)),
            /* new Vector2(0, 0),  //  */new Vector2(1, GetNeighborOpacity(height - 1, x, y, HexSide.NorthWest, HexSide.SouthWest)),
            /* new Vector2(0, 0),  //  */new Vector2(1, GetNeighborOpacity(height - 1, x, y, HexSide.SouthWest, HexSide.South)),
            /* new Vector2(0, 0),  //  */new Vector2(1, GetNeighborOpacity(height - 1, x, y, HexSide.North, HexSide.NorthEast)),
            /* new Vector2(0, 0),  //  */new Vector2(1, GetNeighborOpacity(height - 1, x, y, HexSide.North, HexSide.NorthWest)),
            /* new Vector2(0, 0),  //  */new Vector2(1, GetNeighborOpacity(height - 1, x, y, HexSide.NorthEast, HexSide.SouthEast)),
            /* new Vector2(0, 0),  //  */new Vector2(GetNeighborOpacity(height, x, y, HexSide.NorthEast), GetNeighborOpacity(height, x, y, HexSide.SouthEast)),
            /* new Vector2(0, 0),  //  */new Vector2(GetNeighborOpacity(height, x, y, HexSide.South), GetNeighborOpacity(height, x, y, HexSide.SouthEast)),
            /* new Vector2(0, 0),  //  */new Vector2(GetNeighborOpacity(height, x, y, HexSide.North), GetNeighborOpacity(height, x, y, HexSide.NorthEast)),
            /* new Vector2(0, 0),  //  */new Vector2(GetNeighborOpacity(height, x, y, HexSide.NorthWest), GetNeighborOpacity(height, x, y, HexSide.SouthWest)),
            /* new Vector2(0, 0),  //  */new Vector2(GetNeighborOpacity(height, x, y, HexSide.NorthWest), GetNeighborOpacity(height, x, y, HexSide.North)),
            /* new Vector2(0, 0),  //  */new Vector2(GetNeighborOpacity(height, x, y, HexSide.SouthWest), GetNeighborOpacity(height, x, y, HexSide.South)),
        };

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
    }

    private float GetNeighborOpacity(int height, int x, int y, params HexSide[] sides)
    {
        foreach (HexSide side in sides)
        {
            var n = Helpers.GetNeighborPosition(x, y, side);
            OverworldMapPoint point = CurrentSegment.GetPoint(n);

            if (point != null && point.Height > height)
            {
                return 1.0f;
            }
        }

        return 0f;
    }

    public HexagonMono GetHex(Vector2Int pos)
    {
        if (Helpers.IsInBounds(pos, this.Dimensions) == false)
        {
            return null;
        }

        return Hexagons[pos.x, pos.y];
    }

    public Building GetBuilding(Vector2Int pos)
    {
        if (!Helpers.IsInBounds(pos, this.Dimensions))
        {
            return null;
        }

        return Buildings[pos.x, pos.y];
    }

    public void AddBuilding(Vector2Int pos, Building building)
    {
        this.Buildings[pos.x, pos.y] = building;
    }

    public void DestroyBuilding(Building building)
    {
        this.Buildings[building.GridPosition.x, building.GridPosition.y] = null;
        Destroy(building.gameObject);
    }
}

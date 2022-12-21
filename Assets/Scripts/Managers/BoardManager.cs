using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Hexagon[,] Board;
    public HexagonMono[,] HexagonMonos;
    public string ActiveMapName;
    private Building[,] Buildings;
    public RectInt Dimensions;
    private Vector2Int Center => this.Dimensions.max / 2;
    public Navigation Navigation;

    void Awake()
    {
        SpawnMap();
    }

    private void SpawnMap()
    {
        this.Board = OverworldTerrainGenerator.GenerateSingleSegment(50, 50, UnityEngine.Random.Range(0, 1000));

        this.Buildings = new Building[Board.GetLength(0), Board.GetLength(1)];
        this.Dimensions = new RectInt(0, 0, Board.GetLength(0), Board.GetLength(1));

        SpawnTownHall();
        SpawnHexagons();

        Navigation = new Navigation(this.Dimensions.max, Center);
        Navigation.ReacalculatePath(this.Board);
    }

    private void SpawnTownHall()
    {
        // flatten area
        var points = new List<Vector2Int> { Center };
        for (int i = 0; i < 6; i++)
        {
            points.Add(Helpers.GetNeighborPosition(Center, (HexSide)i));
        }

        float averageHeight = 0;
        List<Biome> biomesThatArentWater = new List<Biome> { Board[Center.x, Center.y].Biome };
        foreach (Vector2Int point in points)
        {
            averageHeight += Board[point.x, point.y].Height;

            if (Board[point.x, point.y].Biome != Biome.Water)
            {
                biomesThatArentWater.Add(Board[point.x, point.y].Biome);
            }
        }

        averageHeight /= 7;
        int newHeight = (int)Mathf.Round(averageHeight);
        Biome mostCommonHex = biomesThatArentWater
            .GroupBy(q => q)
            .OrderByDescending(gp => gp.Count())
            .First().Key;
        foreach (Vector2Int point in points)
        {
            Board[point.x, point.y] = Prefabs.GetHexagonScript(mostCommonHex, newHeight);
        }

        Building building = InstantiateBuilding(Center, BuildingType.TownHall);
        AddBuilding(Center, building);
    }

    public Building InstantiateBuilding(Vector2Int pos, BuildingType type)
    {
        var buildingGO = GameObject.Instantiate(
            Prefabs.GetBuilding(type),
            Vector3.zero,
            Prefabs.GetBuilding(type).transform.rotation);
        Building building = buildingGO.GetComponent<Building>();
        building.GridPosition = pos;
        buildingGO.transform.position = building.GetWorldPosition();
        return building;
    }

    private void CleanupMap()
    {
        foreach (HexagonMono hexagon in this.HexagonMonos)
        {
            Destroy(hexagon?.gameObject);
        }
    }

    private void SpawnHexagons()
    {
        this.HexagonMonos = new HexagonMono[Board.GetLength(0), Board.GetLength(1)];

        for (int y = 0; y < Board.GetLength(1); y++)
        {
            for (int x = 0; x < Board.GetLength(0); x++)
            {
                if (Board[x, y] == null)
                {
                    continue;
                }

                BuildHexagon(x, y);
            }
        }
    }

    private bool isValidPath(List<Vector2Int> path, Vector2Int expectedStart, Vector2Int expectedEnd)
    {
        return path?.Count > 0 && path[0] == expectedStart && path.Last() == expectedEnd;
    }

    private void BuildHexagon(int x, int y)
    {
        Hexagon point = Board[x, y];
        Vector3 position = Helpers.ToWorldPosition(x, y);
        position.y = point.Height * Constants.HEXAGON_HEIGHT;
        GameObject go = Instantiate(Prefabs.Hexagons[point.Biome], position, new Quaternion(), this.transform);
        HexagonMono hexagonScript = go.GetComponent<HexagonMono>();
        hexagonScript.SetType(Prefabs.GetHexagonScript(point.Biome, point.Height));
        this.HexagonMonos[x, y] = go.GetComponent<HexagonMono>();
        this.HexagonMonos[x, y].GridPosition = new Vector2Int(x, y);
        this.HexagonMonos[x, y].Height = point.Height;
        this.HexagonMonos[x, y].name = point.Biome + "," + point.Height.ToString();
        this.SetSideData(go, point.Height, x, y);
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
            if (!Helpers.IsInBounds(n, Dimensions))
            {
                continue;
            }

            Hexagon point = Board[n.x, n.y];
            if (point.Height > height)
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

        return HexagonMonos[pos.x, pos.y];
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

        foreach (HexSide side in building.ExtraSize)
        {
            Vector2Int extraPos = Helpers.GetNeighborPosition(pos, side);
            this.Buildings[extraPos.x, extraPos.y] = building;
        }

    }

    public void DestroyBuilding(Building building)
    {
        this.Buildings[building.GridPosition.x, building.GridPosition.y] = null;

        foreach (HexSide side in building.ExtraSize)
        {
            Vector2Int extraPos = Helpers.GetNeighborPosition(building.GridPosition, side);
            this.Buildings[extraPos.x, extraPos.y] = null;
        }

        Destroy(building.gameObject);
    }
}

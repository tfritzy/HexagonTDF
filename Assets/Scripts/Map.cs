using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map
{
    public Dictionary<Vector2Int, BuildingType> Buildings;
    public List<Vector2Int> LandableShores;
    public HashSet<Vector2Int> OceanHex;
    public HashSet<Vector2Int> MainLandmass;
    public int Width { get { return hexes.GetLength(0); } }
    public int Height { get { return hexes.GetLength(1); } }
    public bool IsInvalid;
    public float[,] HexHeightMap;

    private HexagonType?[,] hexes;
    private List<HashSet<Vector2Int>> landGroups;

    public const float LAND_PERLIN_SCALE = 5f;
    public const float FORREST_PERLIN_SCALE = 3f;
    public const float LandPerlinCutoff = .65f;
    public const float TreePerlinCutoff = .55f;


    public Map(int width, int height, int islandRadius)
    {
        GenerateMap(width, height, islandRadius);
        Buildings = new Dictionary<Vector2Int, BuildingType>();
    }

    public void GenerateMap(int width, int height, float islandRadius)
    {
        HexagonType?[,] hexes = new HexagonType?[width, height];
        HexHeightMap = new float[width, height];
        int seed = Random.Range(0, 100000);
        int forrestSeed = Random.Range(0, 100000);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float distFromCenter = DistFromCenter(width, height, x, y);
                if (distFromCenter > islandRadius)
                {
                    hexes[x, y] = HexagonType.Water;
                    continue;
                }

                float sampleX = x / LAND_PERLIN_SCALE;
                float sampleY = y / LAND_PERLIN_SCALE;
                float perlinValue = Mathf.PerlinNoise(sampleX + seed, sampleY + seed);
                hexes[x, y] = perlinValue < LandPerlinCutoff ? HexagonType.Grass : HexagonType.Water;

                float treePerlinValue = Mathf.PerlinNoise(x / FORREST_PERLIN_SCALE + forrestSeed, y / FORREST_PERLIN_SCALE + forrestSeed);
                if (treePerlinValue > TreePerlinCutoff)
                {
                    hexes[x, y] = HexagonType.Forrest;
                }

                float heightBias = (-4 / islandRadius) * distFromCenter + 4;
                float heightNoise = perlinValue * 4;
                HexHeightMap[x, y] = (int)(heightBias + heightNoise) / 2;
            }
        }

        SetHexes(hexes);
    }

    private static float DistFromCenter(int width, int height, int x, int y)
    {
        Vector2 vector = new Vector2(x - width / 2, y - height / 2);
        return vector.magnitude;
    }

    public void SetHexes(HexagonType?[,] hexes)
    {
        this.hexes = hexes;
        FindOceanHexes();
        FindMainLandmass();
        ConnectOrphanedLandMasses();
        FindLandableShores();
    }

    public HexagonType? GetHex(int x, int y)
    {
        if (x >= Width || x < 0)
        {
            return null;
        }

        if (y >= Height || y < 0)
        {
            return null;
        }

        return hexes[x, y];
    }

    public HexagonType? GetHex(Vector2Int pos)
    {
        return GetHex(pos.x, pos.y);
    }

    private void FindOceanHexes()
    {
        Vector2Int startingHex = Constants.MinVector2Int;

        // Run across the bottom row looking for a water tile to start on.
        for (int x = 0; x < Width; x++)
        {
            if (GetHex(x, 0).Value == HexagonType.Water)
            {
                startingHex = new Vector2Int(x, 0);
                break;
            }
        }

        if (startingHex == Constants.MinVector2Int)
        {
            IsInvalid = true;
            return;
        }

        this.OceanHex = Helpers.GetCongruentHexes(this, startingHex, (Vector2Int pos) => { return GetHex(pos.x, pos.y).Value == HexagonType.Water; });
    }

    private void FindMainLandmass()
    {
        HashSet<Vector2Int> allTraversableHexes = new HashSet<Vector2Int>();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Prefabs.GetHexagonScript(hexes[x, y].Value).IsWalkable)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    allTraversableHexes.Add(pos);
                }
            }
        }

        landGroups = Helpers.FindCongruentGroups(this, allTraversableHexes, (Vector2Int pos) => { return Prefabs.GetHexagonScript(hexes[pos.x, pos.y].Value).IsWalkable; });

        // Groups are sorted by size descending.
        MainLandmass = landGroups[0];
    }

    private void ConnectOrphanedLandMasses()
    {
        for (int i = 1; i < landGroups.Count; i++)
        {
            if (landGroups[i].Count < 5)
            {
                continue;
            }
            List<Vector2Int> path = Helpers.FindPath(this, landGroups[i].First(), MainLandmass, (Vector2Int pos) => { return GetHex(pos).Value != HexagonType.Water; });

            foreach (Vector2Int point in path)
            {
                if (GetHex(point).Value != HexagonType.Grass)
                {
                    hexes[point.x, point.y] = HexagonType.Grass;
                    MainLandmass.Add(point);
                }
            }

            foreach (Vector2Int point in landGroups[i])
            {
                MainLandmass.Add(point);
            }
        }
    }

    private void FindLandableShores()
    {
        HashSet<Vector2Int> landableShoreSet = new HashSet<Vector2Int>();
        foreach (Vector2Int pos in MainLandmass)
        {
            for (int i = 0; i < 6; i++)
            {
                if (OceanHex.Contains(Helpers.GetNeighborPosition(this, pos, i)))
                {
                    landableShoreSet.Add(pos);
                }
            }
        }

        if (landableShoreSet.Count < 5)
        {
            IsInvalid = true;
            return;
        }

        LandableShores = landableShoreSet.ToList();

        foreach (Vector2Int shore in this.LandableShores)
        {
            hexes[shore.x, shore.y] = HexagonType.Shore;
        }
    }

    public static Vector3 ToWorldPosition(int x, int y)
    {
        float xF = x * Constants.HorizontalDistanceBetweenHexagons;
        float zF = y * Constants.VerticalDistanceBetweenHexagons + (x % 2 == 1 ? Constants.HEXAGON_r : 0);
        return new Vector3(xF, 0f, zF);
    }

    public static Vector3 ToWorldPosition(Vector2Int position)
    {
        return ToWorldPosition(position.x, position.y);
    }
}



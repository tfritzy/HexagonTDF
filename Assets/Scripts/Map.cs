using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map
{
    public Dictionary<Vector2Int, BuildingType> Buildings;
    public Vector2Int Trebuchet;
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
    public const float FORREST_PERLIN_SCALE = 2.5f;
    public const float SHORE_PERLIN_SCALE = 2f;
    public const float LandPerlinCutoff = .70f;
    public const float TreePerlinCutoff = .70f;
    public const float ShorePerlinCutoff = .3f;

    public Map(int width, int height)
    {
        Buildings = new Dictionary<Vector2Int, BuildingType>();
        GenerateMap(width, height);
    }

    public void GenerateMap(int width, int height)
    {
        this.hexes = new HexagonType?[width, height];
        HexHeightMap = new float[width, height];
        int seed = Random.Range(0, 100000);
        int forrestSeed = Random.Range(0, 100000);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float distFromCenter = DistFromCenter(x, y);

                float sampleX = x / LAND_PERLIN_SCALE;
                float sampleY = y / LAND_PERLIN_SCALE;
                float perlinValue = Mathf.PerlinNoise(sampleX + seed, sampleY + seed);
                hexes[x, y] = perlinValue < LandPerlinCutoff ? HexagonType.Grass : HexagonType.Water;

                float treePerlinValue = Mathf.PerlinNoise(x / FORREST_PERLIN_SCALE + forrestSeed, y / FORREST_PERLIN_SCALE + forrestSeed);

                if (treePerlinValue > TreePerlinCutoff)
                {
                    hexes[x, y] = HexagonType.Forrest;
                }

                float heightNoise = (perlinValue) * 3;
                float finalValue = (int)(heightNoise / 2f);
                // HexHeightMap[x, y] = finalValue > 0 ? finalValue : 0;
                HexHeightMap[x, y] = 0;
            }
        }

        ConfigureMap();
    }

    private float DistFromCenter(int x, int y)
    {
        Vector2 vector = new Vector2(x - Width / 2, y - Height / 2);
        return vector.magnitude;
    }

    public void ConfigureMap()
    {
        // FindOceanHexes();
        FindMainLandmass();
        ConnectOrphanedLandMasses();
        // FindLandableShores();
        PlaceVillageBuildings();
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

        foreach (Vector2Int hex in this.OceanHex)
        {
            HexHeightMap[hex.x, hex.y] = 0;
        }
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

            if (path == null)
            {
                continue;
            }

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

    private float shorePerlinNoise(int x, int y)
    {
        int seed = Random.Range(0, 100000);
        float sampleX = x / SHORE_PERLIN_SCALE;
        float sampleY = y / SHORE_PERLIN_SCALE;
        return Mathf.PerlinNoise(sampleX + seed, sampleY + seed);
    }

    private void PlaceVillageBuildings()
    {
        List<Vector2Int> topLandmass = new List<Vector2Int>();
        foreach (Vector2Int pos in MainLandmass)
        {
            if (hexes[pos.x, pos.y] == HexagonType.Grass && pos.y > this.Height * .75f)
            {
                topLandmass.Add(pos);
            }
        }

        this.Trebuchet = new Vector2Int(this.Width / 2, 2);

        float numBarracks = 3;
        for (float i = 0; i < topLandmass.Count; i += topLandmass.Count / numBarracks)
        {
            Vector2Int pos = topLandmass[(int)i];
            Buildings[pos] = BuildingType.Barracks;
        }
    }
}



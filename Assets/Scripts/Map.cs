﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map
{
    public Dictionary<Vector2Int, BuildingType> Buildings;
    public List<Vector2Int> LandableShores;
    public List<Vector2Int> Docks;
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
    public const float LandPerlinCutoff = .60f;
    public const float TreePerlinCutoff = .60f;
    public const int MaxDocks = 6;
    public const int MinDocks = 3;


    public Map(int width, int height, int islandRadius)
    {
        Buildings = new Dictionary<Vector2Int, BuildingType>();
        GenerateMap(width, height, islandRadius);
    }

    public void GenerateMap(int width, int height, float islandRadius)
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

                if (treePerlinValue > TreePerlinCutoff && distFromCenter < islandRadius * .8f)
                {
                    hexes[x, y] = HexagonType.Forrest;
                }

                float heightBias = (-4 / islandRadius) * distFromCenter + 4;
                float heightNoise = -(treePerlinValue - 1) * 2;
                float finalValue = (int)((heightBias + heightNoise) / 2f);
                HexHeightMap[x, y] = finalValue > 0 ? finalValue : 0;
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
        FindOceanHexes();
        FindMainLandmass();
        ConnectOrphanedLandMasses();
        FindLandableShores();
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

        for (int i = 0; i < LandableShores.Count; i++)
        {
            Vector2Int shorePos = LandableShores[i];
            hexes[shorePos.x, shorePos.y] = HexagonType.Shore;
            HexHeightMap[shorePos.x, shorePos.y] = 0;
        }

        Docks = new List<Vector2Int>();
        List<Vector2Int> possibleDocks = new List<Vector2Int>();
        for (int i = 0; i < LandableShores.Count; i += 1)
        {
            if (Buildings.ContainsKey(LandableShores[i]))
            {
                continue;
            }

            int waterCount = 0;
            for (int j = 0; j < 6; j++)
            {
                Vector2Int pos = Helpers.GetNeighborPosition(this, LandableShores[i], j);
                if (hexes[pos.x, pos.y] == HexagonType.Water)
                {
                    waterCount += 1;
                    if (waterCount == 3)
                    {
                        possibleDocks.Add(Helpers.GetNeighborPosition(this, LandableShores[i], j - 1));
                        break;
                    }
                }
                else
                {
                    waterCount = 0;
                }
            }
        }

        if (possibleDocks.Count < MinDocks)
        {
            this.IsInvalid = true;
            return;
        }

        int numDocks = Random.Range(MinDocks, Mathf.Min(MaxDocks + 1, possibleDocks.Count));
        while (numDocks > 0)
        {
            int selectedIndex = Random.Range(0, possibleDocks.Count);
            Docks.Add(possibleDocks[selectedIndex]);
            Buildings[possibleDocks[selectedIndex]] = BuildingType.Dock;
            possibleDocks.RemoveAt(selectedIndex);
            numDocks -= 1;
        }
    }

    private void PlaceVillageBuildings()
    {
        List<Vector2Int> centerLandmass = new List<Vector2Int>();
        foreach (Vector2Int pos in MainLandmass)
        {
            if (hexes[pos.x, pos.y] == HexagonType.Grass && DistFromCenter(pos.x, pos.y) < 5)
            {
                centerLandmass.Add(pos);
            }
        }

        int numHouses = MainLandmass.Count / 20;
        for (int i = 0; i < numHouses; i++)
        {
            int index = Random.Range(0, centerLandmass.Count);
            Vector2Int pos = centerLandmass[index];
            Buildings[pos] = BuildingType.House;
            centerLandmass.RemoveAt(index);
        }

        float numTowers = 5;
        float hexesBetweenTowers = LandableShores.Count / numTowers;
        for (float i = 0; i < LandableShores.Count; i += hexesBetweenTowers)
        {
            Vector2Int pos = LandableShores[(int)i];
            Buildings[pos] = BuildingType.GuardTower;
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



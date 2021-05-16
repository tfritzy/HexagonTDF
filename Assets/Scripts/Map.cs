﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    [JsonProperty]
    public Dictionary<string, BuildingType> Buildings;

    public HashSet<Vector2Int> LandableShores;
    public HashSet<Vector2Int> OceanHex;
    public HashSet<Vector2Int> MainLandmass;
    public int Width { get { return hexes.GetLength(0); } }
    public int Height { get { return hexes.GetLength(1); } }
    public bool IsInvalid;

    private HexagonType?[,] hexes;

    public Map()
    {
        Buildings = new Dictionary<string, BuildingType>();
        LandableShores = new HashSet<Vector2Int>();
    }

    public void SetHexes(HexagonType?[,] hexes)
    {
        this.hexes = hexes;
        FindOceanHexes();
        FindMainLandmass();
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

        List<HashSet<Vector2Int>> groups = Helpers.FindCongruentGroups(this, allTraversableHexes, (Vector2Int pos) => { return Prefabs.GetHexagonScript(hexes[pos.x, pos.y].Value).IsWalkable; });
        foreach (HashSet<Vector2Int> group in groups)
        {
            foreach (Vector2Int pos in group)
            {
                GameObject.Instantiate(Prefabs.PathCorner, Map.ToWorldPosition(pos), new Quaternion());
            }

            Debug.Log(group.Count);
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



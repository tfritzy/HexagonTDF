using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject HexagonPrefab;
    public int BoardSideLength = 30;
    public Hexagon[,] Hexagons;
    public Dictionary<Vector2Int, Building> Buildings;
    public List<Portal> Portals;
    public Building Source;
    public string ActiveMapName;

    void Awake()
    {
        SpawnMap();
    }

    private void SpawnMap()
    {
        Map map = LoadMap();
        SpawnHexagons(map.Hexagons);
        SpawnBuildings(map.Buildings);
    }

    private void SpawnHexagons(HexagonType?[,] hexagonMap)
    {
        this.Hexagons = new Hexagon[hexagonMap.GetLength(0), hexagonMap.GetLength(1)];

        for (int y = 0; y < hexagonMap.GetLength(1); y++)
        {
            for (int x = 0; x < hexagonMap.GetLength(0); x++)
            {
                if (hexagonMap[x, y].HasValue == false)
                {
                    continue;
                }

                GameObject go = Instantiate(HexagonPrefab, Hexagon.ToWorldPosition(x, y), new Quaternion(), this.transform);
                Hexagon hexagonScript = Prefabs.GetHexagonScript(hexagonMap[x, y].Value);
                go.AddComponent(hexagonScript.GetType());
                this.Hexagons[x, y] = go.GetComponent<Hexagon>();
                this.Hexagons[x, y].GridPosition = new Vector2Int(x, y);
            }
        }
    }

    public void AddBuilding(Building building)
    {
        if (Buildings.ContainsKey(building.Position) && Buildings[building.Position] != null)
        {
            throw new System.ArgumentException("Cannot build a building on an occupied spot.");
        }

        Buildings[building.Position] = building;

        foreach (Portal portal in Portals)
        {
            portal.RecalculatePath();
        }
    }

    private void SpawnBuildings(Dictionary<string, BuildingType> buildingMap)
    {
        this.Buildings = new Dictionary<Vector2Int, Building>();
        foreach (string strPosition in buildingMap.Keys)
        {
            string[] posSplits = strPosition.Split(',');
            Vector2Int position = new Vector2Int(int.Parse(posSplits[0]), int.Parse(posSplits[1]));
            Building building = Instantiate(
                    Prefabs.Buildings[buildingMap[strPosition]],
                    Hexagon.ToWorldPosition(position),
                    new Quaternion(),
                    this.transform)
                    .GetComponent<Building>();
            building.Initialize(position);

            if (buildingMap[strPosition] == BuildingType.Source)
            {
                Source = this.Buildings[position];
            }

            if (buildingMap[strPosition] == BuildingType.Portal)
            {
                Portals.Add((Portal)building);
            }
        }
    }

    public HexagonType?[,] GetTypeMap()
    {
        HexagonType?[,] typeMap = new HexagonType?[this.Hexagons.GetLength(0), this.Hexagons.GetLength(1)];
        for (int x = 0; x < this.Hexagons.GetLength(0); x++)
        {
            for (int y = 0; y < this.Hexagons.GetLength(1); y++)
            {
                typeMap[x, y] = this.Hexagons[x, y]?.Type;
            }
        }

        return typeMap;
    }

    private Map LoadMap()
    {
        TextAsset text = Resources.Load<TextAsset>(Constants.FilePaths.Maps + ActiveMapName);
        if (text == null)
        {
            Map newMap = new Map();
            newMap.Hexagons = new HexagonType?[BoardSideLength, BoardSideLength];
            for (int i = 0; i < newMap.Hexagons.GetLength(0); i++)
            {
                for (int j = 0; j < newMap.Hexagons.GetLength(1); j++)
                {
                    newMap.Hexagons[i, j] = HexagonType.Grass;
                }
            }
            return newMap;
        }
        Map map = JsonConvert.DeserializeObject<Map>(text.text);
        return map;
    }

    public bool IsBlockedByBuilding(Vector2Int gridPosition)
    {
        return Buildings.ContainsKey(gridPosition) && Buildings[gridPosition] != null;
    }

    public bool IsBuildable(Vector2Int pos)
    {
        return Hexagons[pos.x, pos.y].IsBuildable && Buildings.ContainsKey(pos) == false;
    }

    public Dictionary<Vector2Int, BuildingType> GetBuildingTypeMap()
    {
        Dictionary<Vector2Int, BuildingType> typeMap = new Dictionary<Vector2Int, BuildingType>();
        foreach (Vector2Int key in Buildings.Keys)
        {
            typeMap[key] = Buildings[key].Type;
        }

        return typeMap;
    }
}

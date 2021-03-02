using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject Hexagon;
    public int BoardSideLength = 30;
    public Hexagon[,] Hexagons;
    public new Dictionary<Vector2Int, Building> Buildings;
    public string ActiveMapName;

    void Start()
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

                GameObject go = Instantiate(Hexagon, GetHexagonPosition(x, y), new Quaternion(), this.transform);
                Hexagon hexagonScript = Prefabs.GetHexagonScript(hexagonMap[x, y].Value);
                go.AddComponent(hexagonScript.GetType());
                this.Hexagons[x, y] = go.GetComponent<Hexagon>();
            }
        }
    }

    private void SpawnBuildings(Dictionary<string, BuildingType> buildingMap)
    {
        this.Buildings = new Dictionary<Vector2Int, Building>();
        foreach (string strPosition in buildingMap.Keys)
        {
            string[] posSplits = strPosition.Split(',');
            Vector2Int position = new Vector2Int(int.Parse(posSplits[0]), int.Parse(posSplits[1]));
            this.Buildings[position] = Instantiate(
                    Prefabs.Buildings[buildingMap[strPosition]],
                    GetHexagonPosition(position.x, position.y),
                    new Quaternion(),
                    this.transform)
                .GetComponent<Building>();
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

    private Vector3 GetHexagonPosition(int x, int y)
    {
        float xF = x * Constants.HorizontalDistanceBetweenHexagons;
        float zF = y * Constants.VerticalDistanceBetweenHexagons + (x % 2 == 1 ? Constants.HEXAGON_r : 0);
        return new Vector3(xF, 0f, zF);
    }

    private Map LoadMap()
    {
        TextAsset text = Resources.Load<TextAsset>(Constants.FilePaths.Maps + ActiveMapName);
        Map map = JsonConvert.DeserializeObject<Map>(text.text);
        return map;
    }
}

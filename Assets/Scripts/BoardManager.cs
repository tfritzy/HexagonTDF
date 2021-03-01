using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject Hexagon;
    public int BoardSideLength = 30;
    public Hexagon[,] Map;
    public string ActiveMapName;

    void Start()
    {
        SpawnBoard();
    }

    private void SpawnBoard()
    {
        HexagonType[,] typeMap = LoadMap();
        this.Map = new Hexagon[typeMap.GetLength(0), typeMap.GetLength(1)];

        for (int y = 0; y < typeMap.GetLength(1); y++)
        {
            for (int x = 0; x < typeMap.GetLength(0); x++)
            {
                GameObject go = Instantiate(Hexagon, GetHexagonPosition(x, y), new Quaternion(), this.transform);
                Hexagon hexagonScript = Prefabs.GetHexagonScript(typeMap[x, y]);
                go.AddComponent(hexagonScript.GetType());
                Map[x, y] = go.GetComponent<Hexagon>();
            }
        }
    }

    public HexagonType[,] GetTypeMap()
    {
        HexagonType[,] typeMap = new HexagonType[Map.GetLength(0), Map.GetLength(1)];
        for (int x = 0; x < Map.GetLength(0); x++)
        {
            for (int y = 0; y < Map.GetLength(1); y++)
            {
                typeMap[x, y] = Map[x, y].Type;
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

    private HexagonType[,] LoadMap()
    {
        if (string.IsNullOrEmpty(ActiveMapName))
        {
            return new HexagonType[BoardSideLength, BoardSideLength];
        }

        TextAsset text = Resources.Load<TextAsset>(Constants.FilePaths.Maps + ActiveMapName);
        Contract.Map map = JsonConvert.DeserializeObject<Contract.Map>(text.text);
        return map.TypeMap;
    }

    public void SetGreyscale()
    {
        foreach (Hexagon hexagon in Map)
        {
            hexagon.SetMaterial(Constants.Materials.Greyscale);
        }
    }
}

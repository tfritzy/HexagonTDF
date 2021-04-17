using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    [JsonProperty]
    public HexagonType?[,] Hexagons;

    [JsonProperty]
    public Dictionary<string, BuildingType> Buildings;

    public Map()
    {
        Buildings = new Dictionary<string, BuildingType>();
    }
}



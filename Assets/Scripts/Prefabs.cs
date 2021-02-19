using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prefabs
{
    private static Dictionary<HexagonType, GameObject> hexagons;
    public static Dictionary<HexagonType, GameObject> Hexagons
    {
        get
        {
            if (hexagons == null)
            {
                hexagons = new Dictionary<HexagonType, GameObject>();

                foreach (HexagonType type in Enum.GetValues(typeof(HexagonType)))
                {
                    hexagons[type] = Resources.Load<GameObject>("Prefabs/Hexagons/" + type.ToString());
                }
            }

            return hexagons;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private static Dictionary<BuildingType, GameObject> buildings;
    public static Dictionary<BuildingType, GameObject> Buildings
    {
        get
        {
            if (buildings == null)
            {
                buildings = new Dictionary<BuildingType, GameObject>();
                foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
                {
                    buildings[buildingType] = Resources.Load<GameObject>("Prefabs/Buildings/" + buildingType);
                }
            }
            return buildings;
        }
    }

    private static Dictionary<BuildingType, Sprite> buildingIcons;
    public static Dictionary<BuildingType, Sprite> BuildingIcons
    {
        get
        {
            if (buildingIcons == null)
            {
                buildingIcons = new Dictionary<BuildingType, Sprite>();
                foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
                {
                    buildingIcons[buildingType] = Resources.Load<Sprite>("Icons/" + buildingType);
                }
            }
            return buildingIcons;
        }
    }

    private static GameObject attackTowerBuildMenu;
    public static GameObject AttackTowerBuildMenu
    {
        get
        {
            if (attackTowerBuildMenu == null)
            {
                attackTowerBuildMenu = Resources.Load<GameObject>("Prefabs/UI/AttackTowerBuildMenu");
            }
            return attackTowerBuildMenu;
        }
    }
}

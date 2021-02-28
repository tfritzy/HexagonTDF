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

    private static Dictionary<UIIconType, Sprite> uIIcons;
    public static Dictionary<UIIconType, Sprite> UIIcons
    {
        get
        {
            uIIcons = new Dictionary<UIIconType, Sprite>();
            foreach (UIIconType iconType in Enum.GetValues(typeof(UIIconType)))
            {
                uIIcons[iconType] = Resources.Load<Sprite>("Icons/" + iconType);
            }
            return uIIcons;
        }
    }

    private static Dictionary<UIElementType, GameObject> uiElements;
    public static Dictionary<UIElementType, GameObject> UIElements
    {
        get
        {
            if (uiElements == null)
            {
                uiElements = new Dictionary<UIElementType, GameObject>();
                foreach (UIElementType uiElementType in Enum.GetValues(typeof(UIElementType)))
                {
                    uiElements[uiElementType] = Resources.Load<GameObject>("Prefabs/UI/" + uiElementType);
                }
            }

            return uiElements;
        }
    }
}

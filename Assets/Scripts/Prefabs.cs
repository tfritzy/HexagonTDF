using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Prefabs
{
    private static Dictionary<HexagonType, GameObject> hexagonModels;
    public static Dictionary<HexagonType, GameObject> HexagonModels
    {
        get
        {
            if (hexagonModels == null)
            {
                hexagonModels = new Dictionary<HexagonType, GameObject>();

                foreach (HexagonType type in Enum.GetValues(typeof(HexagonType)))
                {
                    hexagonModels[type] = Resources.Load<GameObject>("Prefabs/Hexagons/" + type.ToString());
                }
            }

            return hexagonModels;
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

    private static Dictionary<BuildingType, GameObject> projectiles;
    public static Dictionary<BuildingType, GameObject> Projectiles
    {
        get
        {
            if (projectiles == null)
            {
                projectiles = new Dictionary<BuildingType, GameObject>();
                foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
                {
                    projectiles[buildingType] = Resources.Load<GameObject>("Prefabs/Projectiles/" + buildingType);
                }
            }
            return projectiles;
        }
    }

    private static GameObject highlightHex;
    public static GameObject HighlightHex
    {
        get
        {
            if (highlightHex == null)
            {
                highlightHex = Resources.Load<GameObject>("Prefabs/Hexagons/HighlightHexagon");
            }
            return highlightHex;
        }
    }

    private static Dictionary<EnemyType, Enemy> enemies;
    public static Dictionary<EnemyType, Enemy> Enemies
    {
        get
        {
            if (enemies == null)
            {
                enemies = new Dictionary<EnemyType, Enemy>();
                foreach (EnemyType type in Enum.GetValues(typeof(EnemyType)))
                {
                    enemies[type] = Resources.Load<GameObject>("Prefabs/Enemies/" + type).GetComponent<Enemy>();
                }
            }
            return enemies;
        }
    }

    public static Hexagon GetHexagonScript(HexagonType hexagonType)
    {
        switch (hexagonType)
        {
            case (HexagonType.Forrest):
                return new Forrest();
            case (HexagonType.Grass):
                return new Grass();
            case (HexagonType.Stone):
                return new Stone();
            case (HexagonType.Water):
                return new Water();
            default:
                throw new ArgumentException($"Unknown hexagon type: {hexagonType}");
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

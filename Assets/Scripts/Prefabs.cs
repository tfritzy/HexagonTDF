using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Prefabs
{
    private static Dictionary<Biome, GameObject> hexagons;
    public static Dictionary<Biome, GameObject> Hexagons
    {
        get
        {
            if (hexagons == null)
            {
                hexagons = new Dictionary<Biome, GameObject>();

                foreach (Biome type in Enum.GetValues(typeof(Biome)))
                {
                    hexagons[type] = Resources.Load<GameObject>("Prefabs/Hexagons/" + type.ToString());
                }
            }

            return hexagons;
        }
    }

    private static Dictionary<AlliedUnitType, GameObject> allies;
    public static Dictionary<AlliedUnitType, GameObject> Allies
    {
        get
        {
            if (allies == null)
            {
                allies = new Dictionary<AlliedUnitType, GameObject>();

                foreach (AlliedUnitType type in Enum.GetValues(typeof(AlliedUnitType)))
                {
                    allies[type] = Resources.Load<GameObject>("Prefabs/Allies/" + type.ToString());
                }
            }

            return allies;
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

    private static Dictionary<EnemyType, GameObject> enemyprojectiles;
    public static Dictionary<EnemyType, GameObject> EnemyProjectiles
    {
        get
        {
            if (enemyprojectiles == null)
            {
                enemyprojectiles = new Dictionary<EnemyType, GameObject>();
                foreach (EnemyType enemyType in Enum.GetValues(typeof(EnemyType)))
                {
                    enemyprojectiles[enemyType] = Resources.Load<GameObject>("Prefabs/Projectiles/" + enemyType);
                }
            }
            return enemyprojectiles;
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

    public static Hexagon GetHexagonScript(Biome biome)
    {
        switch (biome)
        {
            case (Biome.Sand):
                return new Sand();
            case (Biome.Forrest):
                return new Forrest();
            case (Biome.Grassland):
                return new Grassland();
            case (Biome.Mountain):
                return new Mountain();
            case (Biome.Snow):
                return new Snow();
            case (Biome.Water):
                return new Water();
            default:
                throw new System.Exception("Unknown biome " + biome);
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

    private static Dictionary<ResourceType, Sprite> resourceIcons;
    public static Dictionary<ResourceType, Sprite> ResourceIcons
    {
        get
        {
            if (resourceIcons == null)
            {
                resourceIcons = new Dictionary<ResourceType, Sprite>();
                foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
                {
                    resourceIcons[resourceType] = Resources.Load<Sprite>("Icons/" + resourceType);
                }
            }
            return resourceIcons;
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

    private static GameObject rangeCircle;
    public static GameObject RangeCircle
    {
        get
        {
            if (rangeCircle == null)
            {
                rangeCircle = Resources.Load<GameObject>("Prefabs/Projectiles/RangeCircle");
            }
            return rangeCircle;
        }
    }

    private static GameObject damageNumber;
    public static GameObject DamageNumber
    {
        get
        {
            if (damageNumber == null)
            {
                damageNumber = Resources.Load<GameObject>("Prefabs/UI/DamageNumber");
            }
            return damageNumber;
        }
    }

    private static GameObject resourceNumber;
    public static GameObject ResourceNumber
    {
        get
        {
            if (resourceNumber == null)
            {
                resourceNumber = Resources.Load<GameObject>("Prefabs/UI/ResourceNumber");
            }
            return resourceNumber;
        }
    }

    private static GameObject healthbar;
    public static GameObject Healthbar
    {
        get
        {
            if (healthbar == null)
            {
                healthbar = Resources.Load<GameObject>("Prefabs/UI/Healthbar");
            }
            return healthbar;
        }
    }

    private static GameObject pathCorner;
    public static GameObject PathCorner
    {
        get
        {
            if (pathCorner == null)
            {
                pathCorner = Resources.Load<GameObject>("Prefabs/UI/PathCorner");
            }
            return pathCorner;
        }
    }
}

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

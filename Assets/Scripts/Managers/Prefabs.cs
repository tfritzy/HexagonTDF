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

    private static Dictionary<BuildingType, GameObject> _buildings;
    public static GameObject GetBuilding(BuildingType buildingType)
    {
        if (_buildings == null)
        {
            _buildings = new Dictionary<BuildingType, GameObject>();
        }

        if (_buildings.ContainsKey(buildingType))
        {
            return _buildings[buildingType];
        } else 
        {
            _buildings[buildingType] = Resources.Load<GameObject>($"Prefabs/Buildings/{buildingType}");
            return _buildings[buildingType];
        }
    }

    private static Dictionary<ItemType, GameObject> _resources;
    public static GameObject GetResource(ItemType ItemType)
    {
        if (_resources == null)
        {
            _resources = new Dictionary<ItemType, GameObject>();
        }

        if (_resources.ContainsKey(ItemType))
        {
            return _resources[ItemType];
        } else 
        {
            _resources[ItemType] = Resources.Load<GameObject>($"Prefabs/Resources/{ItemType}");
            return _resources[ItemType];
        }
    }

    private static Dictionary<MaterialType, Material> _materials;
    public static Material GetMaterial(MaterialType materialType)
    {
        if (_materials == null)
        {
            _materials = new Dictionary<MaterialType, Material>();
        }

        if (_materials.ContainsKey(materialType))
        {
            return _materials[materialType];
        } else 
        {
            _materials[materialType] = Resources.Load<Material>($"Materials/{materialType}");
            return _materials[materialType];
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

    private static Dictionary<ItemType, Sprite> resourceIcons;
    public static Dictionary<ItemType, Sprite> ResourceIcons
    {
        get
        {
            if (resourceIcons == null)
            {
                resourceIcons = new Dictionary<ItemType, Sprite>();
                foreach (ItemType ItemType in Enum.GetValues(typeof(ItemType)))
                {
                    resourceIcons[ItemType] = Resources.Load<Sprite>("Icons/" + ItemType);
                }
            }
            return resourceIcons;
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

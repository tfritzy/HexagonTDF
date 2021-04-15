using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const float HEXAGON_r = 0.866f;
    public const float HEXAGON_R = 1.066f;
    public const float HorizontalDistanceBetweenHexagons = HEXAGON_R + HEXAGON_r / 2;
    public const float VerticalDistanceBetweenHexagons = HEXAGON_r * 2;
    public static readonly Vector2Int MinVector2Int = new Vector2Int(int.MinValue, int.MinValue);
    public static readonly Vector2Int MaxVector2Int = new Vector2Int(int.MaxValue, int.MaxValue);

    /// <summary>
    /// How much of each resource 1 power maps to.
    /// </summary>
    public static Dictionary<ResourceType, int> ResourcePowerMap => resourcePowerMap;
    private static Dictionary<ResourceType, int> resourcePowerMap = new Dictionary<ResourceType, int>
        {
            { ResourceType.Wood, 20},
            { ResourceType.Stone, 10},
            { ResourceType.Gold, 5}
        };

    public static class FilePaths
    {
        public const string Maps = "Maps/";
    }

    public static class Layers
    {
        public static int Default = 1 << 0;
        public static int Hexagons = 1 << 8;
        public static int Characters = 1 << 9;
    }

    private static Dictionary<ResourceType, Color> resourceColors = new Dictionary<ResourceType, Color>()
    {
        { ResourceType.Gold, ColorExtensions.Create("F8C21C")},
        { ResourceType.Wood, ColorExtensions.Create("B99C6B")},
        { ResourceType.Stone, ColorExtensions.Create("BECFCC")},
    };
    public static Dictionary<ResourceType, Color> ResourceColors => resourceColors;

    public static class Tags
    {
        public const string Hexagon = "Hexagon";
    }

    public static class Materials
    {
        private static Material greyscale;
        public static Material Greyscale
        {
            get
            {
                if (greyscale == null)
                {
                    greyscale = Resources.Load<Material>("Materials/Greyscale");
                }
                return greyscale;
            }
        }

        private static Material frozen;
        public static Material Frozen
        {
            get
            {
                if (frozen == null)
                {
                    frozen = Resources.Load<Material>("Materials/FrozenColorPalette");
                }
                return frozen;
            }
        }


        private static Material normal;
        public static Material Normal
        {
            get
            {
                if (normal == null)
                {
                    normal = Resources.Load<Material>("Materials/ColorPalette");
                }
                return normal;
            }
        }

        private static Material blueSeethrough;
        public static Material BlueSeethrough
        {
            get
            {
                if (blueSeethrough == null)
                {
                    blueSeethrough = Resources.Load<Material>("Materials/BlueSeethrough");
                }
                return blueSeethrough;
            }
        }

        private static Material redSeethrough;
        public static Material RedSeethrough
        {
            get
            {
                if (redSeethrough == null)
                {
                    redSeethrough = Resources.Load<Material>("Materials/RedSeethrough");
                }
                return redSeethrough;
            }
        }

        private static Material gold;
        public static Material Gold
        {
            get
            {
                if (gold == null)
                {
                    gold = Resources.Load<Material>("Materials/Gold");
                }
                return gold;
            }
        }
    }
}

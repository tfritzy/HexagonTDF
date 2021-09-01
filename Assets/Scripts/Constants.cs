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
    public static Vector3 CenterScreen => new Vector3(Screen.width / 2, Screen.height / 2);
    public const float ENEMY_HEALTH_PER_POWER = 50;
    public const float ENEMY_DEFAULT_MOVEMENTSPEED = 1f;
    // The duration used for balance calculations such as how much damage a tower should do in an interval, or how many enemies to spawn.
    public const float BALANCE_INTERVAL_SECONDS = 15f;
    public const float HARD_DIFFICULTY_ADJUSTMENT = .8f;
    public const int BASE_CITY_META_POWER = 10;

    /// <summary>
    /// How much of each resource 1 power maps to.
    /// </summary>
    public static Dictionary<ResourceType, float> ResourcePowerMap => resourcePowerMap;
    private static Dictionary<ResourceType, float> resourcePowerMap = new Dictionary<ResourceType, float>
        {
            { ResourceType.Gold, 50},
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
        public static int Ground = 1 << 11;
        public static int UI = 1 << 5;
    }

    private static Dictionary<ResourceType, Color> resourceColors = new Dictionary<ResourceType, Color>()
    {
        { ResourceType.Gold, ColorExtensions.Create("F8C21C")},
        { ResourceType.Gem, ColorExtensions.Create("1B80E2")},
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

        private static Material tintableHex;
        public static Material TintableHex
        {
            get
            {
                if (tintableHex == null)
                {
                    tintableHex = Resources.Load<Material>("Materials/TintableHex");
                }
                return tintableHex;
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

        private static Material tintableUnlit;
        public static Material TintableUnlit
        {
            get
            {
                if (tintableUnlit == null)
                {
                    tintableUnlit = Resources.Load<Material>("Materials/TintableUnlit");
                }
                return tintableUnlit;
            }
        }
    }
}

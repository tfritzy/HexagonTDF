using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const float HEXAGON_r = 0.866f;
    public const float HEXAGON_R = 1.066f;
    public const float HEXAGON_HEIGHT = .35f;
    public const float OVERWORLD_HEXAGON_R = .15f;
    public const int OVERWORLD_DIMENSIONS = 25;
    public const float HorizontalDistanceBetweenHexagons = HEXAGON_R + HEXAGON_r / 2;
    public const float VerticalDistanceBetweenHexagons = HEXAGON_r * 2;
    public static readonly float OverworldHorizontalDistanceBetweenHexagons = Constants.OVERWORLD_HEXAGON_R + Mathf.Sin(30.0f * Mathf.Deg2Rad) * Constants.OVERWORLD_HEXAGON_R;
    public static readonly float OverworldVerticalDistanceBetweenHexagons = 2.0f * Mathf.Cos(30.0f * Mathf.Deg2Rad) * Constants.OVERWORLD_HEXAGON_R;
    public static readonly Vector2Int MinVector2Int = new Vector2Int(int.MinValue / 2, int.MinValue / 2);
    public static readonly Vector2Int MaxVector2Int = new Vector2Int(MAX_ISH, MAX_ISH);
    public const int MAX_ISH = int.MaxValue;
    public const int MIN_ISH = int.MinValue;
    public const string AnimationStateParameter = "AnimationState";

    public class Colors
    {
        public static Color Red = ColorExtensions.Create("#FF2F2F");
        public static Color Green = ColorExtensions.Create("#3EF857");
    }

    /// <summary>
    /// How much of each resource 1 power maps to.
    /// </summary>

    public static class FilePaths
    {
        public const string Maps = "Maps/";
    }

    public static class Layers
    {
        public const int Default = 1 << 0;
        public const int Hexagons = 1 << 8;
        public const int Buildings = 1 << BuildingsLayerIndex;
        public const int Units = 1 << 10;
        public const int UI = 1 << 5;
        public const int Characters = Buildings | Units;

        public const int BuildingsLayerIndex = 9;
    }

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

        private static Material overworldColorPalette;
        public static Material OverworldColorPalette
        {
            get
            {
                if (overworldColorPalette == null)
                {
                    overworldColorPalette = Resources.Load<Material>("Materials/OverworldColorPalette");
                }
                return overworldColorPalette;
            }
        }
    }
}

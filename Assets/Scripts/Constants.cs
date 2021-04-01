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
    }
}

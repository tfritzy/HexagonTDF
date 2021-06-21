
using System;
using UnityEngine;

public static class ColorExtensions
{
    public static Color Create(string colorHexCode, int a = 255)
    {
        if (colorHexCode.StartsWith("#"))
        {
            colorHexCode = colorHexCode.Split('#')[1];
        }

        int r = Convert.ToInt32("0x" + colorHexCode.Substring(0, 2), 16);
        int g = Convert.ToInt32("0x" + colorHexCode.Substring(2, 2), 16);
        int b = Convert.ToInt32("0x" + colorHexCode.Substring(4, 2), 16);
        return ColorExtensions.Create(r, g, b, a);
    }

    public static Color Create(int r, int g, int b, int a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static Color Lighten(Color color)
    {
        float a = color.a;
        color *= 1.5f;
        color.a = a;
        return color;
    }

    public static Color WithAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    public static Color RandomlyVary(Color color, float maxVariance)
    {
        float value = UnityEngine.Random.Range(-maxVariance, maxVariance);
        Color.RGBToHSV(color, out float h, out float s, out float v);
        return Color.HSVToRGB(h + value, s, v + value);
    }

    public static Color VaryBy(Color color, float amount)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        return Color.HSVToRGB(h, s, v + amount);
    }
}
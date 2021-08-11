using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandGenerator : MonoBehaviour
{
    const int DIMENSIONS = 2048;

    public double Scale;
    public int Octaves;
    public double Persistence;
    public double Lacunarity;

    public float SnowLine;
    public float WaterLine;
    public float TreeLine;
    public float LushMoistureLine;
    public float GrasslandsMoistureLine;
    public bool ShouldRegenerate;
    public Vector3 stuff;

    private enum Biome
    {
        Snow,
        Mountain,
        Forrest,
        Grassland,
        Sand,
        Water,
    }

    private Dictionary<Biome, Color> colorMap = new Dictionary<Biome, Color>
    {
        {Biome.Snow, ColorExtensions.Create("#dee1e3")},
        {Biome.Mountain, ColorExtensions.Create("#80857e")},
        {Biome.Forrest, ColorExtensions.Create("#6a7132")},
        {Biome.Grassland, ColorExtensions.Create("#94a050")},
        {Biome.Sand, ColorExtensions.Create("#c6ba8a")},
        {Biome.Water, ColorExtensions.Create("#5f8b8e")},
    };

    // Start is called before the first frame update
    void Start()
    {
        ShouldRegenerate = true;
    }

    public void ResetTexture()
    {
        OpenSimplexNoise heightNoise = new OpenSimplexNoise();
        OpenSimplexNoise moistureNoise = new OpenSimplexNoise(DateTime.Now.Ticks - 5);
        Texture2D mapTexture = new Texture2D(DIMENSIONS, DIMENSIONS, TextureFormat.ARGB32, false);
        Texture2D heightTexture = new Texture2D(DIMENSIONS, DIMENSIONS, TextureFormat.ARGB32, false);
        Texture2D moistureTexture = new Texture2D(DIMENSIONS, DIMENSIONS, TextureFormat.ARGB32, false);
        for (int y = 0; y < DIMENSIONS; y++)
        {
            for (int x = 0; x < DIMENSIONS; x++)
            {
                double xD = x / Scale;
                double yD = y / Scale;
                float heightValue = (float)heightNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
                heightValue = (heightValue + 1) / 2;
                heightTexture.SetPixel(x, y, new Color(heightValue, heightValue, heightValue));

                float moistureValue = (float)moistureNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
                moistureValue = (moistureValue + 1) / 2;
                moistureValue = (moistureValue * .7f) + (heightValue * .3f);
                moistureTexture.SetPixel(x, y, new Color(0, 0, 1, moistureValue));

                mapTexture.SetPixel(x, y, GetMapPixel(heightValue, moistureValue));
            }
        }
        heightTexture.Apply();
        moistureTexture.Apply();
        mapTexture.Apply();
        this.transform.Find("HeightMap").GetComponent<RawImage>().texture = heightTexture;
        this.transform.Find("MoistureMap").GetComponent<RawImage>().texture = moistureTexture;
        this.transform.Find("Map").GetComponent<RawImage>().texture = mapTexture;
    }

    private Color GetMapPixel(float height, float moisture)
    {
        if (height < WaterLine)
        {
            return colorMap[Biome.Water];
        }
        else if (height > SnowLine)
        {
            return colorMap[Biome.Snow];
        }
        else if (height > TreeLine)
        {
            return colorMap[Biome.Mountain];
        }
        else if (moisture > LushMoistureLine)
        {
            return colorMap[Biome.Forrest];
        }
        else if (moisture > GrasslandsMoistureLine)
        {
            return colorMap[Biome.Grassland];
        }
        else
        {
            return colorMap[Biome.Sand];
        }
    }

    void Update()
    {
        if (ShouldRegenerate)
        {
            ResetTexture();
            ShouldRegenerate = false;
        }
    }
}

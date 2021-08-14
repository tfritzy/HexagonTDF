using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandGenerator : MonoBehaviour
{
    const int DIMENSIONS = 1024;
    private float halfDimensions = DIMENSIONS / 2f;
    private float quarterDimensionsFloat = DIMENSIONS / 4f;
    private int quarterDimensionsInt = DIMENSIONS / 4;

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

    private Dictionary<Biome, Color> colorMap = new Dictionary<Biome, Color>
    {
        {Biome.Snow, ColorExtensions.Create("#dee1e3")},
        {Biome.Mountain, ColorExtensions.Create("#80857e")},
        {Biome.Forrest, ColorExtensions.Create("#6a7132")},
        {Biome.Grassland, ColorExtensions.Create("#98a050")},
        {Biome.Sand, ColorExtensions.Create("#c6ba8a")},
        {Biome.Water, ColorExtensions.Create("#5f8b8e")},
    };

    public struct MapPoint
    {
        public float Height;
        public Biome Biome;
    }

    void Start()
    {
        ShouldRegenerate = true;
    }

    public MapPoint[,] GetSegment(int index, int Seed)
    {
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(Seed);
        OpenSimplexNoise moistureNoise = new OpenSimplexNoise(Seed + 1);
        MapPoint[,] mapSegment = new MapPoint[DIMENSIONS, DIMENSIONS];
        int yOffset = DIMENSIONS * index;
        for (int y = yOffset; y < DIMENSIONS + yOffset; y++)
        {
            for (int x = 0; x < DIMENSIONS; x++)
            {
                double xD = x / Scale;
                double yD = y / Scale;
                float heightValue = (float)heightNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
                heightValue = (heightValue + 1) / 2;
                heightValue -= trimEdgesModification(x);

                float moistureValue = (float)moistureNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
                moistureValue = (moistureValue + 1) / 2;
                moistureValue = (moistureValue * .6f) + (heightValue * .4f);

                mapSegment[x, y - yOffset] = new MapPoint
                {
                    Height = heightValue,
                    Biome = GetBiome(heightValue, moistureValue),
                };
            }
        }

        return mapSegment;
    }

    private float trimEdgesModification(int x)
    {
        float dist = Mathf.Abs(x - halfDimensions) / halfDimensions;

        if (dist < .5f)
        {
            return 0;
        }
        else
        {
            return dist - .5f;
        }
    }

    private Biome GetBiome(float height, float moisture)
    {
        if (height < WaterLine)
        {
            return Biome.Water;
        }
        else if (height > SnowLine)
        {
            return Biome.Snow;
        }
        else if (height > TreeLine)
        {
            return Biome.Mountain;
        }
        else if (moisture > LushMoistureLine)
        {
            return Biome.Forrest;
        }
        else if (moisture > GrasslandsMoistureLine)
        {
            return Biome.Grassland;
        }
        else
        {
            return Biome.Sand;
        }
    }

    public Texture2D GetTextureOfMap(MapPoint[,] mapPoints)
    {
        Texture2D texture = new Texture2D(DIMENSIONS, DIMENSIONS, TextureFormat.ARGB32, false);

        for (int y = 0; y < mapPoints.GetLength(1); y++)
        {
            for (int x = 0; x < mapPoints.GetLength(0); x++)
            {
                texture.SetPixel(x, y, colorMap[mapPoints[x, y].Biome]);
            }
        }

        texture.Apply();
        return texture;
    }

}

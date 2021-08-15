using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class IslandGenerator : MonoBehaviour
{
    const int DIMENSIONS = 128;
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

    private struct BiomeFormationCriterium
    {
        public Biome Biome;
        public float MinMoisture;
    }

    private Dictionary<float, BiomeFormationCriterium[]> biomeDeterminator = new Dictionary<float, BiomeFormationCriterium[]>
    {
        {
            .9f,
            new BiomeFormationCriterium[]
            {
                new BiomeFormationCriterium {Biome = Biome.Snow, MinMoisture = float.MinValue}
            }
        },
        {
            0.82f,
            new BiomeFormationCriterium[]
            {
                new BiomeFormationCriterium {Biome = Biome.Mountain, MinMoisture = float.MinValue}
            }
        },
        {
            0.45f,
            new BiomeFormationCriterium[]
            {
                new BiomeFormationCriterium {Biome = Biome.Forrest, MinMoisture = .57f},
                new BiomeFormationCriterium {Biome = Biome.Grassland, MinMoisture = .46f},
                new BiomeFormationCriterium {Biome = Biome.Sand, MinMoisture = float.MinValue},
            }
        },
        {
            -0.1f,
            new BiomeFormationCriterium[]
            {
                new BiomeFormationCriterium {Biome = Biome.Water, MinMoisture = float.MinValue},
            }
        },
    };

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
        {Biome.Null, Color.magenta},
    };

    public struct MapPoint
    {
        public float Height;
        public float Moisture;
        public Biome Biome;
        public float HeightDiffFromMinReq;
    }

    void Start()
    {
        ShouldRegenerate = true;
    }

    public async Task<MapPoint[,]> GetSegment(int xIndex, int yIndex, int Seed)
    {
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(Seed);
        OpenSimplexNoise moistureNoise = new OpenSimplexNoise(Seed + 1);
        MapPoint[,] mapSegment = new MapPoint[DIMENSIONS, DIMENSIONS];
        int xOffset = DIMENSIONS * xIndex;
        int yOffset = DIMENSIONS * yIndex;

        var list = new List<Task>();
        for (var y = yOffset; y < DIMENSIONS + yOffset; y++)
        {
            var yCopy = y;
            var t = new Task(() =>
                {
                    formatRow(mapSegment, heightNoise, moistureNoise, yCopy, xOffset, yOffset);
                });
            list.Add(t);
            t.Start();
        }

        await Task.WhenAll(list);

        return mapSegment;
    }

    private void formatRow(MapPoint[,] mapSegment, OpenSimplexNoise heightNoise, OpenSimplexNoise moistureNoise, int y, int xOffset, int yOffset)
    {
        for (int x = xOffset; x < DIMENSIONS + xOffset; x++)
        {
            double xD = x / Scale;
            double yD = y / Scale;
            float heightValue = (float)heightNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
            heightValue = (heightValue + 1) / 2;
            // heightValue -= trimEdgesModification(x + xOffset);

            float moistureValue = (float)moistureNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
            moistureValue = (moistureValue + 1) / 2;
            moistureValue = (moistureValue * .6f) + (heightValue * .4f);

            Tuple<Biome, float> biomeDetails = GetBiome(heightValue, moistureValue);
            mapSegment[x - xOffset, y - yOffset] = new MapPoint
            {
                Height = heightValue,
                Biome = biomeDetails.Item1,
                HeightDiffFromMinReq = biomeDetails.Item2,
                Moisture = moistureValue,
            };
        }

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

    private Tuple<Biome, float> GetBiome(float height, float moisture)
    {
        foreach (float heightLine in this.biomeDeterminator.Keys)
        {
            if (height > heightLine)
            {
                foreach (BiomeFormationCriterium criterium in this.biomeDeterminator[heightLine])
                {
                    if (moisture > criterium.MinMoisture)
                    {
                        return new Tuple<Biome, float>(criterium.Biome, height - heightLine);
                    }
                }
            }
        }

        return new Tuple<Biome, float>(Biome.Null, 0);
    }

    private Dictionary<Biome, float> biomeColorDampens = new Dictionary<Biome, float>()
    {
        {Biome.Water, .33333f},
    };

    public Texture2D GetTextureOfMap(MapPoint[,] mapPoints)
    {
        Texture2D texture = new Texture2D(DIMENSIONS, DIMENSIONS, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < mapPoints.GetLength(1); y++)
        {
            for (int x = 0; x < mapPoints.GetLength(0); x++)
            {
                Color color = colorMap[mapPoints[x, y].Biome];
                float dampen = biomeColorDampens.ContainsKey(mapPoints[x, y].Biome) ? biomeColorDampens[mapPoints[x, y].Biome] : .5f;
                color = ColorExtensions.VaryBy(color, mapPoints[x, y].HeightDiffFromMinReq * dampen);

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

}

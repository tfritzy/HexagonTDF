using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class OverworldTerrainGenerator : MonoBehaviour
{
    public const int DIMENSIONS = 64;
    private const float CITY_CHANCE = .1f;
    private readonly int CITY_LOW_BOUNDS = DIMENSIONS / 5;
    private readonly int CITY_HIGH_BOUNDS = DIMENSIONS - (DIMENSIONS / 5);
    private float halfDimensions = DIMENSIONS / 2f;
    private readonly float CITY_HEIGHT_CUTOFF_DELTA = .02f;

    public double Scale;
    public int Octaves;
    public double Persistence;
    public double Lacunarity;

    private struct BiomeFormationCriterion
    {
        public Biome Biome;
        public float MinMoisture;
    }

    private struct BiomeCriteria
    {
        public float Height;
        public BiomeFormationCriterion[] Criteria;
    }

    private List<BiomeCriteria> biomeDeterminator = new List<BiomeCriteria>
    {
        new BiomeCriteria{
            Height = .75f,
            Criteria = new BiomeFormationCriterion[]
            {
                new BiomeFormationCriterion {Biome = Biome.Snow, MinMoisture = float.MinValue}
            }
        },
        new BiomeCriteria{
            Height = 0.65f,
            Criteria = new BiomeFormationCriterion[]
            {
                new BiomeFormationCriterion {Biome = Biome.Mountain, MinMoisture = float.MinValue}
            }
        },
        new BiomeCriteria{
            Height = 0.45f,
            Criteria = new BiomeFormationCriterion[]
            {
                new BiomeFormationCriterion {Biome = Biome.Forrest, MinMoisture = .57f},
                new BiomeFormationCriterion {Biome = Biome.Grassland, MinMoisture = .46f},
                new BiomeFormationCriterion {Biome = Biome.Sand, MinMoisture = float.MinValue},
            }
        },
        new BiomeCriteria{
            Height = -3f,
            Criteria = new BiomeFormationCriterion[]
            {
                new BiomeFormationCriterion {Biome = Biome.Water, MinMoisture = float.MinValue},
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

    public struct Segment
    {
        public MapPoint[,] Points;
        public Vector2Int? CityPosition;
    }

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

    public async Task<Segment> GetSegment(int xIndex, int yIndex, int Seed, float xSlope = 0, float xB = 1)
    {
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(Seed);
        OpenSimplexNoise moistureNoise = new OpenSimplexNoise(Seed + 1);
        Segment segment = new Segment
        {
            Points = new MapPoint[DIMENSIONS, DIMENSIONS],
            CityPosition = null,
        };

        int xOffset = DIMENSIONS * xIndex;
        int yOffset = DIMENSIONS * yIndex;

        var list = new List<Task>();
        for (var y = yOffset; y < DIMENSIONS + yOffset; y++)
        {
            var yCopy = y;
            var t = new Task(() =>
                {
                    formatRow(segment, heightNoise, moistureNoise, yCopy, xOffset, yOffset, xSlope, xB);
                });
            list.Add(t);
            t.Start();
        }

        await Task.WhenAll(list);

        float averageHeight = 0;
        foreach (MapPoint point in segment.Points)
        {
            averageHeight += point.Height;
        }
        averageHeight /= (DIMENSIONS * DIMENSIONS);

        Vector2Int? cityPosition = GetCityPosition(xIndex, yIndex);
        if (cityPosition != null && averageHeight > biomeDeterminator[biomeDeterminator.Count - 2].Height + CITY_HEIGHT_CUTOFF_DELTA)
        {
            segment.CityPosition = new Vector2Int(cityPosition.Value.x, cityPosition.Value.y);
        }

        return segment;
    }

    private Vector2Int? GetCityPosition(int xIndex, int yIndex)
    {
        int citySeed = xIndex * 3 + yIndex * 7;
        System.Random random = new System.Random(citySeed);
        if (random.NextDouble() > CITY_CHANCE)
        {
            return null;
        }

        return new Vector2Int(random.Next(CITY_LOW_BOUNDS, CITY_HIGH_BOUNDS), random.Next(CITY_LOW_BOUNDS, CITY_HIGH_BOUNDS));
    }

    private void formatRow(Segment segment, OpenSimplexNoise heightNoise, OpenSimplexNoise moistureNoise, int y, int xOffset, int yOffset, float xSlope, float xB)
    {
        for (int x = xOffset; x < DIMENSIONS + xOffset; x++)
        {
            double xD = x / Scale;
            double yD = y / Scale;
            float heightValue = (float)heightNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
            heightValue = (heightValue + 1) / 2;
            heightValue *= ((float)(x - xOffset) / (DIMENSIONS) * xSlope + xB);

            float moistureValue = (float)moistureNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
            moistureValue = (moistureValue + 1) / 2;
            moistureValue = (moistureValue * .6f) + (heightValue * .4f);

            Tuple<Biome, float> biomeDetails = GetBiome(heightValue, moistureValue);
            segment.Points[x - xOffset, y - yOffset] = new MapPoint
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
        foreach (BiomeCriteria criteria in biomeDeterminator)
        {
            if (height > criteria.Height)
            {
                foreach (BiomeFormationCriterion criterion in criteria.Criteria)
                {
                    if (moisture > criterion.MinMoisture)
                    {
                        return new Tuple<Biome, float>(criterion.Biome, height - criteria.Height);
                    }
                }
            }
        }

        return new Tuple<Biome, float>(Biome.Null, 0);
    }

    public Texture2D GetTextureOfMap(Segment segment)
    {
        Texture2D texture = new Texture2D(DIMENSIONS, DIMENSIONS, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < segment.Points.GetLength(1); y++)
        {
            for (int x = 0; x < segment.Points.GetLength(0); x++)
            {
                Color color = colorMap[segment.Points[x, y].Biome];

                if (segment.Points[x, y].Biome != Biome.Water)
                {
                    color = ColorExtensions.VaryBy(color, segment.Points[x, y].HeightDiffFromMinReq * .5f);
                }
                else
                {
                    color = ColorExtensions.VaryBy(color, Mathf.Max(segment.Points[x, y].Height - .3f, .08f));
                }

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

}

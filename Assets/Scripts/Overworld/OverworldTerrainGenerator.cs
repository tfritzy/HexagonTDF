using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OverworldTerrainGenerator : MonoBehaviour
{
    public const int DIMENSIONS = 2048;
    private const int SUBDIVISION_SIZE = DIMENSIONS / 8;
    private const float CITY_CHANCE = .2f;
    private readonly int CITY_LOW_BOUNDS = DIMENSIONS / 5;
    private readonly int CITY_HIGH_BOUNDS = DIMENSIONS - (DIMENSIONS / 5);
    private float halfDimensions = DIMENSIONS / 2f;
    private readonly float CITY_HEIGHT_CUTOFF_DELTA = .02f;
    private const int FALLOFF_NEAR_START = 128;
    private const int FALLOFF_FAR_START = DIMENSIONS - FALLOFF_NEAR_START;
    private System.Random random;
    private int Seed;

    public float Scale;
    public int Octaves;
    public float Persistence;
    public float Lacunarity;

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
                new BiomeFormationCriterion {Biome = Biome.Forrest, MinMoisture = .52f},
                new BiomeFormationCriterion {Biome = Biome.Grassland, MinMoisture = .45f},
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

    private Dictionary<Biome, Color> colorMap = new Dictionary<Biome, Color>
    {
        {Biome.Snow, ColorExtensions.Create("#cfd4d8")},
        {Biome.Mountain, ColorExtensions.Create("#728e9b")},
        {Biome.Forrest, ColorExtensions.Create("#6a914c")},
        {Biome.Grassland, ColorExtensions.Create("#7ba659")},
        {Biome.Sand, ColorExtensions.Create("#e1c59f")},
        {Biome.Water, new Color(0, 0, 0, 0)},
        {Biome.Null, Color.magenta},
    };

    public void Initialize(int seed)
    {
        this.random = new System.Random(seed);
        this.Seed = seed;
    }

    public OverworldSegment GetSegment(int index)
    {
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(this.Seed);
        OpenSimplexNoise moistureNoise = new OpenSimplexNoise(this.Seed + 1);
        OverworldSegment segment = new OverworldSegment
        {
            Fortresses = new List<Vector2Int>(),
            Points = new OverworldMapPoint[DIMENSIONS, DIMENSIONS],
            Texture = null,
        };

        var list = new List<Task>();
        for (var y = 0; y < DIMENSIONS; y++)
        {
            var yCopy = y;
            var t = new Task(() =>
                {
                    formatRow(segment.Points, heightNoise, moistureNoise, yCopy);
                });
            list.Add(t);
            t.Start();
        }

        Task.WhenAll(list).Wait();

        for (int x = 0; x < DIMENSIONS / SUBDIVISION_SIZE; x++)
        {
            for (int y = 0; y < DIMENSIONS / SUBDIVISION_SIZE; y++)
            {
                if (DoesSegmentHaveFortress(x, y, segment.Points))
                {
                    segment.Fortresses.Add(
                        new Vector2Int(
                            SUBDIVISION_SIZE * x + SUBDIVISION_SIZE / 2 + random.Next(-50, 50),
                            SUBDIVISION_SIZE * y + SUBDIVISION_SIZE / 2 + random.Next(-50, 50)));
                }
            }
        }

        return segment;
    }

    private bool DoesSegmentHaveFortress(int xSeg, int ySeg, OverworldMapPoint[,] fullMap)
    {
        float averageHeight = 0;
        for (int x = xSeg * SUBDIVISION_SIZE; x < (xSeg + 1) * SUBDIVISION_SIZE; x++)
        {
            for (int y = ySeg * SUBDIVISION_SIZE; y < (ySeg + 1) * SUBDIVISION_SIZE; y++)
            {
                averageHeight += fullMap[x, y].Height;
            }
        }
        averageHeight /= (SUBDIVISION_SIZE * SUBDIVISION_SIZE);

        if (averageHeight > biomeDeterminator[biomeDeterminator.Count - 2].Height + CITY_HEIGHT_CUTOFF_DELTA)
        {
            return true;
        }

        return false;
    }

    private void formatRow(
        OverworldMapPoint[,] points,
        OpenSimplexNoise heightNoise,
        OpenSimplexNoise moistureNoise,
        int y)
    {
        for (int x = 0; x < DIMENSIONS; x++)
        {
            float xD = x / Scale;
            float yD = y / Scale;
            float heightValue = (float)heightNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
            heightValue = (heightValue + 1) / 2;
            heightValue *= CalculateHeightFalloff(x, y);

            float moistureValue = (float)moistureNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
            moistureValue = (moistureValue + 1) / 2;
            moistureValue = (moistureValue * .6f) + (heightValue * .4f);

            Biome biome = GetBiome(heightValue, moistureValue);
            points[x, y] = new OverworldMapPoint
            {
                Height = heightValue,
                Biome = biome,
                Moisture = moistureValue,
            };
        }
    }

    private float CalculateHeightFalloff(int x, int y)
    {
        float progressAlongFalloff = 0;
        if (x < FALLOFF_NEAR_START || x > FALLOFF_FAR_START)
        {
            progressAlongFalloff = Math.Max(x - FALLOFF_FAR_START, FALLOFF_NEAR_START - x);
        }

        if (y < FALLOFF_NEAR_START || y > FALLOFF_FAR_START)
        {
            progressAlongFalloff = Math.Max(y - FALLOFF_FAR_START, FALLOFF_NEAR_START - y);
        }

        if (progressAlongFalloff > 0)
        {
            return 1 - (float)progressAlongFalloff / (float)FALLOFF_NEAR_START;
        }

        return 1f;
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
        foreach (BiomeCriteria criteria in biomeDeterminator)
        {
            if (height > criteria.Height)
            {
                foreach (BiomeFormationCriterion criterion in criteria.Criteria)
                {
                    if (moisture > criterion.MinMoisture)
                    {
                        return criterion.Biome;
                    }
                }
            }
        }

        return Biome.Null;
    }

    public Texture2D GetTextureOfMap(OverworldMapPoint[,] points)
    {
        Texture2D texture = new Texture2D(DIMENSIONS, DIMENSIONS, TextureFormat.RGBAHalf, false);
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < points.GetLength(1); y++)
        {
            for (int x = 0; x < points.GetLength(0); x++)
            {
                if (points[x, y].Biome != Biome.Water)
                {
                    texture.SetPixel(x, y, colorMap[points[x, y].Biome]);
                }
            }
        }

        texture.Apply();
        return texture;
    }
}

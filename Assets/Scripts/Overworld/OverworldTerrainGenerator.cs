﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OverworldTerrainGenerator : MonoBehaviour
{
    public OverworldSegment Segment;
    public Texture2D Texture;
    public string GenerationStep;
    public float GenerationProgress;
    public bool IsComplete;
    public const int DIMENSIONS = 1536;
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

    private static class States
    {
        public const string GENERATING_TERRAIN = "Generating Terrain";
        public const string LOCATING_FORTRESSES = "Locating Fortresses";
        public const string GENERATING_TEXTURE = "Generating Island Texture";
        public const string CALCULATING_TERRITORY_BOUNDS = "Calculating Territory Bounds";
        public const string GENERATING_TERRITORY_TEXTURES = "Generating Territory Textures";
    }

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

    public IEnumerator GenerateSegment(int index)
    {
        IsComplete = false;
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(this.Seed);
        OpenSimplexNoise moistureNoise = new OpenSimplexNoise(this.Seed + 1);
        this.Segment = new OverworldSegment
        {
            FortressIds = new List<string>(),
            FortressPositions = new Dictionary<string, Vector2Int>(),
            Points = new OverworldMapPoint[DIMENSIONS, DIMENSIONS],
            Texture = null,
        };
        this.GenerationStep = States.GENERATING_TERRAIN;

        for (int y = 0; y < DIMENSIONS; y++)
        {
            formatRow(Segment.Points, heightNoise, moistureNoise, y);
            this.GenerationProgress = (float)y / DIMENSIONS;

            if (y % 10 == 0)
            {
                yield return null;
            }
        }

        int numChunks = DIMENSIONS / SUBDIVISION_SIZE;
        this.GenerationStep = States.LOCATING_FORTRESSES;
        int fortressIndex = 0;
        for (int x = 0; x < numChunks; x++)
        {
            for (int y = 0; y < numChunks; y++)
            {
                if (DoesSegmentHaveFortress(x, y, Segment.Points))
                {
                    string id = $"Fortress-{index}-{fortressIndex}";
                    fortressIndex += 1;
                    Segment.FortressIds.Add(id);
                    Segment.FortressPositions[id] =
                        new Vector2Int(
                            SUBDIVISION_SIZE * x + SUBDIVISION_SIZE / 2 + random.Next(-50, 50),
                            SUBDIVISION_SIZE * y + SUBDIVISION_SIZE / 2 + random.Next(-50, 50));
                }
            }

            this.GenerationProgress = (float)x / numChunks;
            yield return null;
        }

        yield return GenerateTextureOfMap();

        yield return CalculateTerritories();
        IsComplete = true;
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

    public IEnumerator GenerateTextureOfMap()
    {
        this.GenerationStep = States.GENERATING_TEXTURE;
        this.Texture = new Texture2D(DIMENSIONS, DIMENSIONS, TextureFormat.RGBAHalf, false);
        this.Texture.filterMode = FilterMode.Point;

        for (int y = 0; y < Segment.Points.GetLength(1); y++)
        {
            for (int x = 0; x < Segment.Points.GetLength(0); x++)
            {
                if (Segment.Points[x, y].Biome != Biome.Water)
                {
                    this.Texture.SetPixel(x, y, colorMap[Segment.Points[x, y].Biome]);
                }
            }

            if (y % 20 == 0 || y == Segment.Points.GetLength(1))
            {
                this.GenerationProgress = (float)y / Segment.Points.GetLength(1);
                yield return null;
            }
        }

        this.Texture.Apply();
    }

    private IEnumerator CalculateTerritories()
    {
        this.GenerationStep = States.CALCULATING_TERRITORY_BOUNDS;
        int dimensions = OverworldTerrainGenerator.DIMENSIONS;
        int numPixels = OverworldTerrainGenerator.DIMENSIONS * OverworldTerrainGenerator.DIMENSIONS;
        var visited = new string[dimensions, dimensions];
        var edges = new Dictionary<string, HashSet<Vector2Int>>();
        var queues = new Dictionary<string, Queue<Vector2Int>>();
        var territoryPoints = new Dictionary<string, List<Vector2Int>>();
        foreach (string fortressId in Segment.FortressIds)
        {
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(Segment.FortressPositions[fortressId]);
            queues[fortressId] = queue;
            visited[Segment.FortressPositions[fortressId].x, Segment.FortressPositions[fortressId].y] = fortressId;
            edges[fortressId] = new HashSet<Vector2Int>();
            territoryPoints[fortressId] = new List<Vector2Int>();
        }

        List<string> finishedFortresses = new List<string>(0);
        int numIterations = 0;
        while (queues.Count > 0)
        {
            foreach (string fortressId in queues.Keys)
            {
                var queue = queues[fortressId];

                if (queue.Count == 0)
                {
                    finishedFortresses.Add(fortressId);
                    continue;
                }

                Vector2Int current = queue.Dequeue();
                numIterations += 1;
                territoryPoints[fortressId].Add(current);
                bool isBorder = false;
                Helpers.GetNonHexGridNeighbors(current, dimensions, (int x, int y) =>
                {
                    if (visited[x, y] == null && Segment.Points[x, y].Biome != Biome.Water)
                    {
                        queue.Enqueue(new Vector2Int(x, y));
                        visited[x, y] = fortressId;
                    }
                    else if (visited[x, y] != null &&
                             visited[x, y] != fortressId ||
                             Segment.Points[x, y].Biome == Biome.Water)
                    {
                        isBorder = true;
                    }

                    return false;
                });

                if (isBorder)
                {
                    edges[fortressId].Add(current);
                }
            }

            foreach (string fortress in finishedFortresses)
            {
                queues.Remove(fortress);
            }
            finishedFortresses = new List<string>();

            if (numIterations % 500 == 0)
            {
                this.GenerationProgress = (float)numIterations / (numPixels * .5f);
                yield return null;
            }
        }

        this.GenerationStep = States.GENERATING_TERRITORY_TEXTURES;
        Segment.Territories = new Dictionary<string, OverworldTerritory>();
        float index = 0;
        foreach (string fortress in territoryPoints.Keys)
        {
            index += 1;
            this.GenerationProgress = index / Segment.Territories.Count;
            Segment.Territories[fortress] = new OverworldTerritory();
            Segment.Territories[fortress].Points = territoryPoints[fortress];
            Segment.Territories[fortress].Edges = edges[fortress];
            yield return null;
        }
    }
}

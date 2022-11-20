﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OverworldTerrainGenerator : MonoBehaviour
{
    public Func<string> GetStatus;
    public Func<float> GetProgress;
    public OverworldSegment Segment;
    public bool IsComplete;
    public GameObject territoryBoundsLR;
    private const int SUBDIVISION_SIZE = Constants.OVERWORLD_DIMENSIONS / 10;
    private float halfDimensions = Constants.OVERWORLD_DIMENSIONS / 2f;
    private readonly float CITY_HEIGHT_CUTOFF_DELTA = .02f;
    private const int FALLOFF_NEAR_START = 5;
    private const int FALLOFF_FAR_START = Constants.OVERWORLD_DIMENSIONS - FALLOFF_NEAR_START;
    private System.Random random;
    private int Seed;
    private HexGridGenerator hexGridGenerator;
    private string GenerationStep;
    private float GenerationProgress;

    public const float Scale = 25;
    public const int Octaves = 5;
    public const float Persistence = .55f;
    public const float Lacunarity = 3;

    private static class States
    {
        public const string GENERATING_TERRAIN = "Generating terrain";
        public const string LOCATING_FORTRESSES = "Locating fortresses";
        public const string GENERATING_MESH = "Generating mesh";
        public const string MAPPING_TO_TEXTURE_ATLAS = "Mapping UVs to texture";
        public const string CALCULATING_TERRITORY_BOUNDS = "Calculating territory bounds";
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

    private static List<BiomeCriteria> biomeDeterminator = new List<BiomeCriteria>
    {
        // new BiomeCriteria{
        //     Height = .75f,
        //     Criteria = new BiomeFormationCriterion[]
        //     {
        //         new BiomeFormationCriterion {Biome = Biome.Snow, MinMoisture = float.MinValue}
        //     }
        // },
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

    private Dictionary<Biome, Vector2Int> colorAtlasMap = new Dictionary<Biome, Vector2Int>()
    {
        {Biome.Snow, new Vector2Int(1, 2)},
        {Biome.Mountain, new Vector2Int(0, 2)},
        {Biome.Forrest, new Vector2Int(0, 0)},
        {Biome.Grassland, new Vector2Int(0, 1)},
        {Biome.Sand, new Vector2Int(2, 2)},
        {Biome.Water, new Vector2Int(2, 0)},
    };

    private Dictionary<Alliances, Vector2Int> allianceAtlasMap = new Dictionary<Alliances, Vector2Int>()
    {
        {Alliances.Maltov, new Vector2Int(1, 0)},
        {Alliances.Player, new Vector2Int(1, 1)},
    };

    public void Initialize(int seed)
    {
        this.random = new System.Random(seed);
        this.Seed = seed;
    }

    public IEnumerator GenerateSegment(int index, string containerName)
    {
        this.GetStatus = () => this.GenerationStep;
        this.GetProgress = () => this.GenerationProgress;
        IsComplete = false;
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(this.Seed);
        OpenSimplexNoise moistureNoise = new OpenSimplexNoise(this.Seed + 1);
        this.Segment = new OverworldSegment
        {
            FortressIds = new List<string>(),
            FortressPositions = new Dictionary<string, Vector2Int>(),
            Points = new OverworldMapPoint[Constants.OVERWORLD_DIMENSIONS, Constants.OVERWORLD_DIMENSIONS],
            FortressAlliances = new Dictionary<string, Alliances>(),
            Index = index,
        };
        this.GenerationStep = States.GENERATING_TERRAIN;

        for (int y = 0; y < Constants.OVERWORLD_DIMENSIONS; y++)
        {
            formatRow(Segment.Points, heightNoise, moistureNoise, Constants.OVERWORLD_DIMENSIONS, y);
            if (y % 10 == 0)
            {
                this.GenerationProgress = (float)y / Constants.OVERWORLD_DIMENSIONS;
                yield return null;
            }
        }

        yield return GenerateMesh(containerName);

        yield return FindFotressLocations(index);

        // yield return CalculateTerritories();
        IsComplete = true;
    }

    public static OverworldSegment GenerateSingleSegment(int width, int height, int seed)
    {
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(seed);
        OpenSimplexNoise moistureNoise = new OpenSimplexNoise(seed + 1);

        var segment = new OverworldSegment
        {
            FortressIds = new List<string>(),
            FortressPositions = new Dictionary<string, Vector2Int>(),
            Points = new OverworldMapPoint[width, height],
            FortressAlliances = new Dictionary<string, Alliances>(),
            Index = 0,
        };

        for (int y = 0; y < height; y++)
        {
            formatRowForSelfConainedSegment(segment.Points, heightNoise, moistureNoise, width, y);
        }

        return segment;
    }

    private IEnumerator GenerateMesh(string containerName)
    {
        this.hexGridGenerator = this.GetComponent<HexGridGenerator>().GetComponent<HexGridGenerator>();
        this.GetStatus = () => this.hexGridGenerator.State;
        this.GetProgress = () => this.hexGridGenerator.Progress;
        yield return this.hexGridGenerator.GenerateMesh(Constants.OVERWORLD_DIMENSIONS, containerName);

        this.GetStatus = () => this.GenerationStep;
        this.GetProgress = () => this.GenerationProgress;
        this.GenerationStep = States.MAPPING_TO_TEXTURE_ATLAS;
        for (int x = 0; x < Constants.OVERWORLD_DIMENSIONS; x++)
        {
            for (int y = 0; y < Constants.OVERWORLD_DIMENSIONS; y++)
            {
                Vector2Int colorPos = colorAtlasMap[Segment.Points[x, y].Biome];
                hexGridGenerator.SetUV(y, x, colorPos.y, colorPos.x);
            }

            this.GenerationProgress = (float)x / Constants.OVERWORLD_DIMENSIONS;
            yield return null;
        }
    }

    private IEnumerator FindFotressLocations(int segmentIndex)
    {
        int numChunks = Constants.OVERWORLD_DIMENSIONS / SUBDIVISION_SIZE;
        this.GenerationStep = States.LOCATING_FORTRESSES;

        for (int x = 0; x < numChunks; x++)
        {
            for (int y = 0; y < numChunks; y++)
            {
                if (DoesSegmentHaveFortress(x, y, Segment.Points))
                {
                    Vector2Int fortressPos =
                        new Vector2Int(
                                SUBDIVISION_SIZE * x + SUBDIVISION_SIZE / 2 + random.Next(-5, 5),
                                SUBDIVISION_SIZE * y + SUBDIVISION_SIZE / 2 + random.Next(-5, 5));

                    if (this.Segment.Points[fortressPos.x, fortressPos.y].Biome != Biome.Water)
                    {
                        string id = $"Fortress-{segmentIndex}-{Segment.FortressIds.Count}";
                        Segment.FortressAlliances[id] =
                            Segment.FortressIds.Count == 0 ?
                            Alliances.Player :
                            Alliances.Maltov;
                        Segment.FortressIds.Add(id);
                        Segment.FortressPositions[id] = fortressPos;

                    }
                }
            }

            this.GenerationProgress = (float)x / numChunks;
            yield return null;
        }
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

    private static void formatRow(
        OverworldMapPoint[,] points,
        OpenSimplexNoise heightNoise,
        OpenSimplexNoise moistureNoise,
        int length,
        int y)
    {
        for (int x = 0; x < length; x++)
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
            };
        }
    }

    private static void formatRowForSelfConainedSegment(
        OverworldMapPoint[,] points,
        OpenSimplexNoise heightNoise,
        OpenSimplexNoise moistureNoise,
        int length,
        int y)
    {
        for (int x = 0; x < length; x++)
        {
            float xD = x / Scale;
            float yD = y / Scale;
            float heightValue = (float)heightNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
            heightValue += .65f;

            float moistureValue = (float)moistureNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
            moistureValue = (moistureValue + 1) / 2;
            moistureValue = (moistureValue * .6f) + (heightValue * .4f);

            Biome biome = GetBiome(heightValue, moistureValue);
            points[x, y] = new OverworldMapPoint
            {
                Height = heightValue,
                Biome = biome,
            };
        }
    }

    private static float CalculateHeightFalloff(int x, int y)
    {
        float progressAlongFalloff = 0;
        if (x < FALLOFF_NEAR_START || x > FALLOFF_FAR_START)
        {
            progressAlongFalloff = Math.Max(x - FALLOFF_FAR_START, FALLOFF_NEAR_START - x);
        }
        else if (y < FALLOFF_NEAR_START || y > FALLOFF_FAR_START)
        {
            progressAlongFalloff = Math.Max(y - FALLOFF_FAR_START, FALLOFF_NEAR_START - y);
        }

        if (progressAlongFalloff > 0)
        {
            return 1 - (float)progressAlongFalloff / (float)FALLOFF_NEAR_START;
        }

        return 1f;
    }

    private static float DistFromCenter(int x, int y, OverworldMapPoint[,] points)
    {
        float progressAlongX = Mathf.Abs(points.GetLength(0) / 2 - x) / (points.GetLength(0) / 2f);
        float progressAlongY = Mathf.Abs(points.GetLength(1) / 2 - y) / (points.GetLength(1) / 2f);
        return Mathf.Max(progressAlongX, progressAlongY);
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

    private static Biome GetBiome(float height, float moisture)
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

    private IEnumerator CalculateTerritories()
    {
        this.GenerationStep = States.CALCULATING_TERRITORY_BOUNDS;
        int dimensions = Constants.OVERWORLD_DIMENSIONS;
        int numHexes = dimensions * dimensions;
        var visited = new string[dimensions, dimensions];
        var edges = new Dictionary<Alliances, HashSet<Vector2Int>>();
        var queues = new Dictionary<string, Queue<Vector2Int>>();
        var territoryPoints = new Dictionary<Alliances, List<Vector2Int>>();
        foreach (string fortressId in Segment.FortressIds)
        {
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(Segment.FortressPositions[fortressId]);
            queues[fortressId] = queue;
            visited[Segment.FortressPositions[fortressId].x,
                    Segment.FortressPositions[fortressId].y] = fortressId;
            edges[Segment.FortressAlliances[fortressId]] = new HashSet<Vector2Int>();
            territoryPoints[Segment.FortressAlliances[fortressId]] = new List<Vector2Int>();
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
                territoryPoints[Segment.FortressAlliances[fortressId]].Add(current);
                bool isBorder = false;

                for (int i = 0; i < 6; i++)
                {
                    Vector2Int neighbor = Helpers.GetNeighborPosition(current, i, dimensions);

                    if (neighbor == Constants.MinVector2Int)
                        continue;

                    if (visited[neighbor.x, neighbor.y] == null && Segment.Points[neighbor.x, neighbor.y].Biome != Biome.Water)
                    {
                        queue.Enqueue(new Vector2Int(neighbor.x, neighbor.y));
                        visited[neighbor.x, neighbor.y] = fortressId;
                    }
                    else if (string.IsNullOrEmpty(visited[neighbor.x, neighbor.y]) == false &&
                             Segment.FortressAlliances[visited[neighbor.x, neighbor.y]] != Segment.FortressAlliances[fortressId])
                    {
                        isBorder = true;
                    }
                }

                if (isBorder)
                {
                    edges[Segment.FortressAlliances[fortressId]].Add(current);
                }
            }

            foreach (string fortress in finishedFortresses)
            {
                queues.Remove(fortress);
            }
            finishedFortresses = new List<string>();

            if (numIterations % 500 == 0)
            {
                this.GenerationProgress = (float)numIterations / (numHexes * .5f);
                yield return null;
            }
        }

        Segment.Territories = new Dictionary<Alliances, OverworldTerritory>();
        float index = 0;
        foreach (Alliances alliance in territoryPoints.Keys)
        {
            index += 1;
            this.GenerationProgress = index / Segment.Territories.Count;
            Segment.Territories[alliance] = new OverworldTerritory();
            Segment.Territories[alliance].Edges = edges[alliance];

            foreach (Vector2Int edge in edges[alliance])
            {
                Vector2Int colorPos = allianceAtlasMap[alliance];
                this.hexGridGenerator.SetUV(edge.y, edge.x, colorPos.y, colorPos.x);
            }

            yield return null;
        }
    }
}

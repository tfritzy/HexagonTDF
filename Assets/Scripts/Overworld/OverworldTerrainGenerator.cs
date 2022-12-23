using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OverworldTerrainGenerator : MonoBehaviour
{
    public Func<string> GetStatus;
    public Func<float> GetProgress;
    public Hexagon[,] Segment;
    public bool IsComplete;
    public GameObject territoryBoundsLR;
    private const int SUBDIVISION_SIZE = Constants.OVERWORLD_DIMENSIONS / 10;
    private float halfDimensions = Constants.OVERWORLD_DIMENSIONS / 2f;
    private const int FALLOFF_NEAR_START = 5;
    private const int FALLOFF_FAR_START = Constants.OVERWORLD_DIMENSIONS - FALLOFF_NEAR_START;
    private System.Random random;
    private int Seed;
    private HexGridGenerator hexGridGenerator;
    private string GenerationStep;
    private float GenerationProgress;

    public const float Scale = 50;
    public const int Octaves = 5;
    public const float Persistence = .55f;
    public const float Lacunarity = 10;

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
            Height = 0.4f,
            Criteria = new BiomeFormationCriterion[]
            {
                new BiomeFormationCriterion {Biome = Biome.Forrest, MinMoisture = .1f},
                new BiomeFormationCriterion {Biome = Biome.Grassland, MinMoisture = .1f},
                new BiomeFormationCriterion {Biome = Biome.Grassland, MinMoisture = .1f},
            }
        },
        new BiomeCriteria{
            Height = 0.3f,
            Criteria = new BiomeFormationCriterion[]
            {
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
        {Biome.Invalid, Color.magenta},
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

    private Dictionary<Alliance, Vector2Int> allianceAtlasMap = new Dictionary<Alliance, Vector2Int>()
    {
        {Alliance.Maltov, new Vector2Int(1, 0)},
        {Alliance.Player, new Vector2Int(1, 1)},
    };

    public void Initialize(int seed)
    {
        this.random = new System.Random(seed);
        this.Seed = seed;
    }

    public static Hexagon[,] GenerateSingleSegment(int width, int height, int seed)
    {
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(seed);
        OpenSimplexNoise moistureNoise = new OpenSimplexNoise(seed + 1);

        var segment = new Hexagon[width, height];
        System.Random random = new System.Random(seed);
        for (int y = 0; y < height; y++)
        {
            formatRowForSelfConainedSegment(segment, heightNoise, moistureNoise, width, y, random);
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
                Vector2Int colorPos = colorAtlasMap[Segment[x, y].Biome];
                hexGridGenerator.SetUV(y, x, colorPos.y, colorPos.x);
            }

            this.GenerationProgress = (float)x / Constants.OVERWORLD_DIMENSIONS;
            yield return null;
        }
    }

    private static void formatRowForSelfConainedSegment(
        Hexagon[,] points,
        OpenSimplexNoise heightNoise,
        OpenSimplexNoise moistureNoise,
        int length,
        int y,
        System.Random random)
    {
        for (int x = 0; x < length; x++)
        {
            float xD = x / Scale;
            float yD = y / Scale;
            float heightValue = .8f + ((float)heightNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity) - .5f) * .75f;
            // float xDistFromCenter = Mathf.Abs(x - points.GetLength(0) / 2) / (points.GetLength(0) / 2f);
            // float yDistFromCenter = Mathf.Abs(y - points.GetLength(1) / 2) / (points.GetLength(1) / 2f);
            // float maxDist = 1 - Math.Max(xDistFromCenter, yDistFromCenter);
            // heightValue *= maxDist;
            // Debug.Log(heightValue + "," + maxDist);

            float moistureValue = (float)moistureNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
            moistureValue = (moistureValue + 1) / 2;
            moistureValue = (moistureValue * .6f) + (heightValue * .4f);

            Biome biome = GetBiome(heightValue, moistureValue, random);
            points[x, y] = Prefabs.GetHexagonScript(biome, (int)(heightValue * 5));
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

    private static Biome GetBiome(float height, float moisture, System.Random random)
    {
        foreach (BiomeCriteria criteria in biomeDeterminator)
        {
            List<Biome> matches = new List<Biome>();
            if (height > criteria.Height)
            {
                foreach (BiomeFormationCriterion criterion in criteria.Criteria)
                {
                    if (moisture > criterion.MinMoisture)
                    {
                        matches.Add(criterion.Biome);
                    }
                }
            }

            if (matches.Count > 0)
            {
                return matches[random.Next(0, matches.Count)];
            }
        }

        return Biome.Invalid;
    }
}

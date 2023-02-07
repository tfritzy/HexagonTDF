using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TerrainGenerator
{
    public Chunk GeneratedChunk;
    public Hexagon[,] Segment;
    private float halfDimensions = Constants.CHUNK_SIZE / 2f;
    private System.Random random;
    private int Seed;
    private HexGridGenerator hexGridGenerator;

    public const float Scale = 25;
    public const int Octaves = 5;
    public const float Persistence = .55f;
    public const float Lacunarity = 2;

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
        // new BiomeCriteria{
        //     Height = 0.65f,
        //     Criteria = new BiomeFormationCriterion[]
        //     {
        //         new BiomeFormationCriterion {Biome = Biome.Mountain, MinMoisture = float.MinValue}
        //     }
        // },
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
            Height = -3f,
            Criteria = new BiomeFormationCriterion[]
            {
                new BiomeFormationCriterion {Biome = Biome.Sand, MinMoisture = float.MinValue},
            }
        },
        // new BiomeCriteria{
        //     Height = -3f,
        //     Criteria = new BiomeFormationCriterion[]
        //     {
        //         new BiomeFormationCriterion {Biome = Biome.Water, MinMoisture = float.MinValue},
        //     }
        // },
    };

    public void GenerateChunk(Vector2Int chunk, int seed, Transform container)
    {
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(seed);
        OpenSimplexNoise moistureNoise = new OpenSimplexNoise(seed + 1);

        var segment = new Hexagon[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, Constants.MAX_HEIGHT];
        int[,] tops = new int[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];
        System.Random random = new System.Random(seed);
        int yOffset = chunk.y * Constants.CHUNK_SIZE;
        int xOffset = chunk.x * Constants.CHUNK_SIZE;

        for (int y = yOffset; y < yOffset + Constants.CHUNK_SIZE; y++)
        {
            for (int x = xOffset; x < xOffset + Constants.CHUNK_SIZE; x++)
            {
                float xD = x / Scale;
                float yD = y / Scale;
                float heightValue = .6f + heightNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
                int height = (int)(heightValue * 5) + 10;
                Helpers.WorldToChunkPos(x, y, height, out Vector2Int chunkIndex, out Vector3Int subPos);
                tops[subPos.x, subPos.y] = height;

                float moistureValue = (float)moistureNoise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
                moistureValue = (moistureValue + 1) / 2;

                Biome biome = GetBiome(heightValue, moistureValue, random);
                segment[subPos.x, subPos.y, height] = Prefabs.GetHexagonScript(biome);

                for (int iHeight = height + 1; iHeight <= Constants.WATER_HEIGHT; iHeight++)
                {
                    segment[subPos.x, subPos.y, iHeight] = Prefabs.GetHexagonScript(Biome.Water);
                }

                // Fill in ground with stone.
                height -= 1;
                while (height >= 0)
                {
                    segment[x - xOffset, y - yOffset, height] = Prefabs.GetHexagonScript(Biome.Stone);
                    height -= 1;
                }
            }
        }

        this.GeneratedChunk = new Chunk(chunk, segment, tops, container);
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

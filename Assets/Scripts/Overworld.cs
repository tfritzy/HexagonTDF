using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overworld : MonoBehaviour
{
    public float HEIGHT_PERLIN_SCALE;
    public float BIOME_PERLIN_SCALE;
    public float WATER_CUTOFF = .3f;
    public bool ShouldUpdate;
    private int Seed = 10293;
    private int worldWidth = 1000;
    private float[,] HeightMap;
    private List<Biome> Biomes = new List<Biome>() {
        Biome.Desert,
        Biome.Grasslands,
        Biome.BlackForest,
        Biome.Mountain,
    };
    private List<float[,]> biomeMaps = new List<float[,]>();

    void Update()
    {
        if (ShouldUpdate)
        {
            ResetTexture();
            ShouldUpdate = false;
        }
    }

    void Start()
    {
        colorMap = new Dictionary<Biome, Color>();
        HeightMap = new float[worldWidth, worldWidth];
        Random.InitState(Seed);
        ResetTexture();
    }

    private void ResetTexture()
    {
        CalculateBiomeMaps();
        Texture2D newTexture = new Texture2D(worldWidth, worldWidth, TextureFormat.ARGB32, false);
        for (int y = 0; y < worldWidth; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                HeightMap[x, y] = GetHeight(x, y);
                newTexture.SetPixel(x, y, GetColorWrapper(GetBiome(x, y), HeightMap[x, y]));
            }
        }
        newTexture.Apply();
        this.GetComponent<MeshRenderer>().material.mainTexture = newTexture;
    }

    private void CalculateBiomeMaps()
    {
        biomeMaps = new List<float[,]>();
        foreach (Biome biome in Biomes)
        {
            float[,] map = new float[worldWidth, worldWidth];
            for (int y = 0; y < worldWidth; y++)
            {
                for (int x = 0; x < worldWidth; x++)
                {
                    map[x, y] = Helpers.PerlinNoise(x, y, BIOME_PERLIN_SCALE, Seed + biomeMaps.Count);
                    map[x, y] = Mathf.Pow(map[x, y], 6f);
                }
            }
            biomeMaps.Add(map);
        }
    }

    private float GetHeight(float x, float y)
    {
        float height = Helpers.PerlinNoise(x, y, HEIGHT_PERLIN_SCALE, Seed);
        height += (x / (worldWidth / 2)) * -.3f + .3f;
        return height;
    }

    public Biome GetBiome(int x, int y)
    {
        if (HeightMap[x, y] < WATER_CUTOFF)
        {
            return Biome.Sea;
        }

        int maxBiomeIndex = 0;
        float maxBiomeValue = biomeMaps[0][x, y];

        for (int i = 1; i < biomeMaps.Count; i++)
        {
            if (biomeMaps[i][x, y] > maxBiomeValue)
            {
                maxBiomeValue = biomeMaps[i][x, y];
                maxBiomeIndex = i;
            }
        }

        return Biomes[maxBiomeIndex];
    }

    private Dictionary<Biome, Color> colorMap;
    private Color GetColorWrapper(Biome biome, float height)
    {
        if (colorMap.ContainsKey(biome) == false)
        {
            colorMap[biome] = GetColor(biome);
        }

        return ColorExtensions.Lighten(colorMap[biome], height - .5f);
    }

    private Color GetColor(Biome biome)
    {
        switch (biome)
        {
            case (Biome.Sea):
                return ColorExtensions.Create("#6798c7");
            case (Biome.Grasslands):
                return ColorExtensions.Create("#90c589");
            case (Biome.Desert):
                return ColorExtensions.Create("#c2b280");
            case (Biome.BlackForest):
                return ColorExtensions.Create("#306847");
            case (Biome.BirchForest):
                return ColorExtensions.Create("#306111");
            case (Biome.Mountain):
                return ColorExtensions.Create("#808080");
            case (Biome.SnowMountain):
                return ColorExtensions.Create("#FFFFFF");
            default:
                return ColorExtensions.Create("#FF00FF");
        }
    }
}

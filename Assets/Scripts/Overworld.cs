using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overworld : MonoBehaviour
{
    public float PERLIN_SCALE = 45f;
    public float SEA_PERLIN_SCALE = 150f;
    public float SEA_NOISE_PERLIN_SCALE = 3f;
    public float SEA_CUTOFF;
    public int Seed = 10293;
    public int worldWidth = 1000;
    public bool ShouldUpdate;

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
        ResetTexture();
    }

    private void ResetTexture()
    {
        Texture2D newTexture = new Texture2D(worldWidth, worldWidth, TextureFormat.ARGB32, false);
        for (int y = 0; y < worldWidth; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                newTexture.SetPixel(x, y, GetColorWrapper(GetBiome(x, y)));
            }
        }
        newTexture.Apply();
        this.GetComponent<MeshRenderer>().material.mainTexture = newTexture;
    }

    private float IsSea(float x, float y)
    {
        float seaValue = Mathf.PerlinNoise(x / SEA_PERLIN_SCALE + Seed, y / SEA_PERLIN_SCALE + Seed) +
                         (Mathf.PerlinNoise(x / SEA_PERLIN_SCALE / 2f + Seed, y / SEA_PERLIN_SCALE / 2f + Seed) - .5f) / 2f +
                         (Mathf.PerlinNoise(x / SEA_PERLIN_SCALE / 4f + Seed, y / SEA_PERLIN_SCALE / 4f + Seed) - .5f) / 4f +
                         (Mathf.PerlinNoise(x / SEA_NOISE_PERLIN_SCALE + Seed, y / SEA_NOISE_PERLIN_SCALE + Seed) - .5f) / 2f;

        return seaValue;
    }

    public Biome GetBiome(float x, float y)
    {
        float seaPerlin = IsSea(x, y);
        if (seaPerlin < SEA_CUTOFF)
        {
            return Biome.Sea;
        }

        if (seaPerlin < SEA_CUTOFF + .05f)
        {
            return Biome.Desert;
        }

        float value = Mathf.PerlinNoise(x / PERLIN_SCALE + Seed, y / PERLIN_SCALE + Seed);

        if (value < .4f)
        {
            return Biome.Grasslands;
        }
        else if (value < .6f)
        {
            return Biome.BlackForest;
        }
        else if (value < .8f)
        {
            return Biome.BirchForest;
        }
        else
        {
            return Biome.Mountain;
        }
    }

    private Dictionary<Biome, Color> colorMap;
    private Color GetColorWrapper(Biome biome)
    {
        if (colorMap.ContainsKey(biome))
        {
            return colorMap[biome];
        }
        else
        {
            colorMap[biome] = GetColor(biome);
            return colorMap[biome];
        }
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
            default:
                return ColorExtensions.Create("#FF00FF");
        }
    }
}

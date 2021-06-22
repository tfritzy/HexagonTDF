using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overworld : MonoBehaviour
{
    public float HEIGHT_PERLIN_SCALE = 45f;
    public int numOctaves = 3;
    public int Seed = 10293;
    public int worldWidth = 1000;
    public float[,] HeightMap;
    public bool ShouldUpdate;
    public List<Biome> Biomes = new List<Biome>() {
        Biome.Sea,
        Biome.Desert,
        Biome.Grasslands,
        Biome.BlackForest,
        Biome.Mountain,
        Biome.SnowMountain,
    };
    public float[] HeightCutoffs = new float[] {
        .3f,
        .35f,
        .4f,
        .7f,
        .9f,
        1.1f
    };

    public List<Biome> AdditionalMaps = new List<Biome>() {
        Biome.BlackForest,
    };

    public float[] AdditionalMapScale = new float[] {
        50f,
    };

    public float[] AdditionalMapWeight = new float[] {
        .3f
    };

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

    private float GetHeight(float x, float y)
    {
        float height = Helpers.PerlinNoise(x, y, HEIGHT_PERLIN_SCALE, Seed, numOctaves);
        height += (x / (worldWidth / 2)) * -.3f + .3f;
        return height;
    }

    public Biome GetBiome(float x, float y)
    {
        float height = GetHeight(x, y);

        for (int i = 0; i < HeightCutoffs.Length; i++)
        {
            if (height > HeightCutoffs[i])
            {
                continue;
            }

            if (i > 0)
            {
                float range = HeightCutoffs[i] - HeightCutoffs[i - 1];
                float progressInRange = height - HeightCutoffs[i - 1];
                float percent = progressInRange / range;
                if (Random.Range(0f, 1f) < percent - getAdditionalMapValue(x, y, Biomes[i]))
                {
                    return Biomes[i];
                }
                else
                {
                    return Biomes[i - 1];
                }
            }
            else
            {
                return Biomes[i];
            }
        }

        if (height < .3f)
        {
            return Biome.Sea;
        }
        else if (height < .35f)
        {
            return Biome.Desert;
        }
        else if (height < .7f)
        {
            return Biome.BlackForest;
        }
        else if (height < .95f)
        {
            return Biome.Mountain;
        }
        else
        {
            return Biome.SnowMountain;
        }
    }

    private float getAdditionalMapValue(float x, float y, Biome biome)
    {
        int index = AdditionalMaps.IndexOf(biome);
        if (index < 0)
        {
            return 0;
        }

        return Mathf.PerlinNoise(
            x / AdditionalMapScale[index] + Seed,
            y / AdditionalMapScale[index] + Seed)
                * AdditionalMapWeight[index];
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

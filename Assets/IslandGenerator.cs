using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandGenerator : MonoBehaviour
{
    const int DIMENSIONS = 2048;

    public double Scale;
    public int Octaves;
    public double Persistence;
    public double Lacunarity;
    public float WaterLine;
    public bool ShouldRegenerate;


    // Start is called before the first frame update
    void Start()
    {
        ShouldRegenerate = true;
    }

    public void ResetTexture()
    {
        OpenSimplexNoise noise = new OpenSimplexNoise();
        Texture2D newTexture = new Texture2D(DIMENSIONS, DIMENSIONS, TextureFormat.ARGB32, false);
        for (int y = 0; y < DIMENSIONS; y++)
        {
            for (int x = 0; x < DIMENSIONS; x++)
            {
                double xD = x / Scale;
                double yD = y / Scale;
                float value = (float)noise.Evaluate(xD, yD, Octaves, Persistence, Lacunarity);
                value = (value + 1) / 2;
                if (value < WaterLine)
                {
                    newTexture.SetPixel(x, y, Color.blue);
                }
                else
                {
                    newTexture.SetPixel(x, y, new Color(value, value, value));
                }

            }
        }
        newTexture.Apply();
        this.transform.Find("Image").GetComponent<RawImage>().texture = newTexture;
    }

    void Update()
    {
        if (ShouldRegenerate)
        {
            ResetTexture();
            ShouldRegenerate = false;
        }
    }
}

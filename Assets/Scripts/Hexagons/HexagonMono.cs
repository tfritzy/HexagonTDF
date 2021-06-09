﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonMono : MonoBehaviour, Interactable
{
    public HexagonType Type { get { return hexagon.Type; } }
    public bool IsBuildable { get { return hexagon.IsBuildable; } }
    public bool IsWalkable { get { return hexagon.IsWalkable; } }
    public Vector2Int GridPosition;

    protected Color ColorAfterVariance;
    protected Hexagon hexagon;
    protected List<MeshRenderer> meshRenderers;

    private const float MAX_COLOR_VARIANCE = .02f;
    private static float ColorVarianceM => (MAX_COLOR_VARIANCE * 2f) / ((float)Managers.Board.Map.Width);
    private MeshRenderer hex;

    public void SetType(Hexagon hexagon)
    {
        this.hexagon = hexagon;
    }

    void Start()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        this.ColorAfterVariance = ColorExtensions.RandomlyVary(this.hexagon.BaseColor, MAX_COLOR_VARIANCE);
        this.hex = transform.Find("Hex")?.GetComponent<MeshRenderer>();
        FindMeshRenderers();
        SetHexBodyColor();
    }

    public void Interact()
    {
        Debug.Log($"Clicked on {Type} hex at {GridPosition}");
    }

    public void SetMaterial(Material material)
    {
        this.hex.material = material;
    }

    public void ResetMaterial()
    {
        SetHexBodyColor();
    }

    private void FindMeshRenderers()
    {
        this.meshRenderers = new List<MeshRenderer>();
        foreach (MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderers.Add(renderer);
        }
    }

    protected virtual void SetHexBodyColor()
    {
        if (this.hex == null)
        {
            return;
        }

        Texture2D newTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        newTexture.SetPixel(0, 0, this.ColorAfterVariance);
        newTexture.Apply();
        this.hex.material = Constants.Materials.TintableHex;
        this.hex.material.mainTexture = newTexture;

        Texture2D darkerTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        darkerTexture.SetPixel(0, 0, ColorExtensions.VaryBy(this.ColorAfterVariance, -.1f));
        darkerTexture.Apply();
        this.hex.materials[1] = Constants.Materials.TintableHex;
        this.hex.materials[1].mainTexture = darkerTexture;
    }
}

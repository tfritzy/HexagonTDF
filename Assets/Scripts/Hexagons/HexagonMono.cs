using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonMono : MonoBehaviour, Interactable
{
    public Biome Biome { get { return hexagon.Biome; } }
    public bool IsBuildable { get { return hexagon.IsBuildable; } }
    public bool IsWalkable { get { return hexagon.IsWalkable; } }
    public Vector2Int GridPosition;

    protected Color ColorAfterVariance;
    protected Hexagon hexagon;
    protected List<MeshRenderer> meshRenderers;
    protected const float MAX_COLOR_VARIANCE = .02f;

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

    public bool Interact()
    {
        bool wasInputUsed = Managers.Board.Hero.InformHexWasClicked(this);

        if (wasInputUsed) return true;

        wasInputUsed = Managers.Board.Hero.InformGameObjectWasClicked(this.gameObject);

        if (wasInputUsed) return true;

        wasInputUsed = Managers.Builder.InformHexWasClicked(this);

        return wasInputUsed;
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
        this.hex.material = this.hexagon.Material;
        this.hex.material.mainTexture = newTexture;

        if (this.hex.materials.Length > 1)
        {
            Texture2D darkerTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            darkerTexture.SetPixel(0, 0, ColorExtensions.VaryBy(this.ColorAfterVariance, -.05f));
            darkerTexture.Apply();
            this.hex.materials[1] = this.hexagon.Material;
            this.hex.materials[1].mainTexture = darkerTexture;
        }
    }
}

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

    private MeshRenderer hexMesh;

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
        this.hexMesh = transform.Find("Hex")?.GetComponent<MeshRenderer>();
        FindMeshRenderers();
        SetHexBodyColor();

        if (hexagon is ObstacleHexagon)
        {
            ((ObstacleHexagon)hexagon).GenerateObstacle(this.transform, this.GridPosition);
        }
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
        this.hexMesh.material = material;
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
        if (this.hexMesh == null)
        {
            return;
        }

        Texture2D newTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        newTexture.SetPixel(0, 0, this.ColorAfterVariance);
        newTexture.Apply();
        Material[] materials = new Material[] { this.hexagon.Material, this.hexagon.Material };
        this.hexMesh.materials = materials;
        this.hexMesh.material.mainTexture = newTexture;

        if (this.hexMesh.materials.Length > 1)
        {
            Texture2D darkerTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            darkerTexture.SetPixel(0, 0, ColorExtensions.VaryBy(this.ColorAfterVariance, -.05f));
            darkerTexture.Apply();
            this.hexMesh.materials[1].mainTexture = darkerTexture;
        }
    }
}

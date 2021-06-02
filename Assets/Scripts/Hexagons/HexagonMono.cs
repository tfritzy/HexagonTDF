using System.Collections;
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

    private const float MAX_COLOR_VARIANCE = .05f;

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
        FindMeshRenderers();
        SetHexBodyColor();
    }

    public void Interact()
    {
        Debug.Log($"Clicked on {Type} hex at {GridPosition}");
    }

    public void SetMaterial(Material material)
    {
        this.gameObject.SetMaterialsRecursively(material);
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
        MeshRenderer hexModel = this.transform.Find("Hex")?.GetComponent<MeshRenderer>();
        if (hexModel == null)
        {
            return;
        }

        Texture2D newTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        this.ColorAfterVariance = ColorExtensions.RandomlyVary(this.hexagon.BaseColor, MAX_COLOR_VARIANCE);
        newTexture.SetPixel(0, 0, this.ColorAfterVariance);
        newTexture.Apply();
        hexModel.material.mainTexture = newTexture;
    }
}

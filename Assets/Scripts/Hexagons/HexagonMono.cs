using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonMono : MonoBehaviour, Interactable
{
    public Biome Biome { get { return hexagon.Biome; } }
    public bool IsBuildable { get { return hexagon.IsBuildable; } }
    public bool IsWalkable { get { return hexagon.IsWalkable; } }
    public Vector2Int GridPosition;
    public int Height;

    protected int colorVaryIndex;
    protected Hexagon hexagon;
    protected List<MeshRenderer> meshRenderers;

    private MeshRenderer hexMesh;
    private MeshRenderer border;
    private static Dictionary<Biome, Dictionary<int, Material[]>> materialCache;

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
        this.hexMesh = transform.Find("hex")?.GetComponent<MeshRenderer>();
        this.border = this.hexMesh.transform.Find("border").GetComponent<MeshRenderer>();
        FindMeshRenderers();
        SetHexBodyColor();
    }

    public void Interact()
    {
        Debug.Log("Clicked on hex at pos " + GridPosition);
    }

    public void MaybeSpawnObstacle(int segmentIndex)
    {
        if (hexagon is ObstacleHexagon)
        {
            ((ObstacleHexagon)hexagon).GenerateObstacle(this.transform, this.GridPosition, segmentIndex);
        }
    }

    public void RemoveObstacle()
    {
        if (this.hexagon is ObstacleHexagon)
        {
            ((ObstacleHexagon)this.hexagon).RemoveObstacle();
        }
    }

    public void SetMaterial(Material material)
    {
        this.hexMesh.material = material;
    }

    public void SetBorderMaterial(Material material)
    {
        this.border.material = material;
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

        if (materialCache == null)
        {
            materialCache = new Dictionary<Biome, Dictionary<int, Material[]>>();
        }

        if (!materialCache.ContainsKey(this.hexagon.Biome))
        {
            materialCache[this.hexagon.Biome] = new Dictionary<int, Material[]>();
        }

        this.colorVaryIndex = 0; //Random.Range(1, 4);

        if (!materialCache[this.hexagon.Biome].ContainsKey(this.colorVaryIndex))
        {
            Color ColorAfterVariance = ColorExtensions.VaryBy(this.hexagon.BaseColor, this.colorVaryIndex * this.hexagon.MaxColorVariance);
            Material newBase = new Material(Prefabs.GetMaterial(MaterialType.Base));
            newBase.color = ColorAfterVariance;
            // // Texture2D newTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            // // newTexture.SetPixel(0, 0, ColorAfterVariance);
            // // newTexture.Apply();
            // // newBase.mainTexture = newTexture;

            Material newHighlight = this.transform.Find("hex/border").GetComponent<MeshRenderer>().material;
            newHighlight.color = ColorExtensions.VaryBy(ColorAfterVariance, -.1f);
            // // Texture2D darkerTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            // // darkerTexture.SetPixel(0, 0, ColorExtensions.VaryBy(ColorAfterVariance, -.15f));
            // // darkerTexture.Apply();
            // // newHighlight.mainTexture = darkerTexture;

            materialCache[this.hexagon.Biome][this.colorVaryIndex] = new Material[] { newBase, newHighlight };
        }

        var materials = materialCache[this.hexagon.Biome][this.colorVaryIndex];
        this.hexMesh.material = materials[0];
        this.transform.Find("hex/border").GetComponent<MeshRenderer>().material = materials[1];
    }
}

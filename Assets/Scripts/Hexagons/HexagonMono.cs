using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonMono : MonoBehaviour, Interactable
{
    public HexagonType Type { get { return hexagon.Type; } }
    public bool IsBuildable { get { return hexagon.IsBuildable; } }
    public bool IsWalkable { get { return hexagon.IsWalkable; } }
    public Vector2Int GridPosition;

    protected Hexagon hexagon;
    protected GameObject model;
    protected List<MeshRenderer> meshRenderers;

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
}

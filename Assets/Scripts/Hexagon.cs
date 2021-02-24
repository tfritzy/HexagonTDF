using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour, Interactable
{
    public HexagonType Type;
    public Vector2Int GridPosition;

    private GameObject model;
    private List<MeshRenderer> meshRenderers;

    void Start()
    {
        SetModel();
        FindMeshRenderers();
    }

    public void Interact()
    {
        if (Managers.Editor != null)
        {
            this.Type = Managers.Editor.SelectedType;
            SetModel();
        }
    }

    private void SetModel()
    {
        Destroy(model);
        this.model = Instantiate(Prefabs.Hexagons[Type], this.transform, false);
    }

    public void SetMaterial(Material material)
    {
        foreach (MeshRenderer renderer in this.meshRenderers)
        {
            renderer.material = material;
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hexagon : MonoBehaviour, Interactable
{
    public abstract HexagonType Type { get; }
    public abstract bool IsBuildable { get; }
    public virtual bool IsWalkable => IsBuildable;
    public Vector2Int GridPosition;

    private GameObject model;
    private List<MeshRenderer> meshRenderers;

    void Start()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        SetModel();
        FindMeshRenderers();
    }

    public void Interact()
    {
        if (Managers.Editor != null)
        {
            SetModel();
        }
    }

    private void SetModel()
    {
        Destroy(model);
        this.model = Instantiate(Prefabs.HexagonModels[Type], this.transform, false);
        this.model.transform.position = this.model.transform.position + Vector3.down;
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

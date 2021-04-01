using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hexagon : MonoBehaviour, Interactable
{
    public abstract HexagonType Type { get; }
    public abstract bool IsBuildable { get; }
    public Vector2Int GridPosition;

    private GameObject model;
    private List<MeshRenderer> meshRenderers;
    private readonly List<Vector2Int> oddNeighborPattern = new List<Vector2Int>()
    {
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 1)
    };

    private readonly List<Vector2Int> evenNeighborPattern = new List<Vector2Int>()
    {
        new Vector2Int(-1,-1),
        new Vector2Int(0, -1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0)
    };

    void Start()
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

    public bool IsTraversable
    {
        get
        {
            return IsBuildable && Managers.BoardManager.IsBlockedByBuilding(this.GridPosition) == false;
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

    public Vector2Int GetNeighborPosition(int index)
    {
        Vector2Int position;

        if (GridPosition.x % 2 == 0)
        {
            position = GridPosition + evenNeighborPattern[index];
        }
        else
        {
            position = GridPosition + oddNeighborPattern[index];
        }

        if (Helpers.IsInBounds(position))
        {
            return position;
        }
        else
        {
            return Constants.MinVector2Int;
        }
    }

    public Hexagon GetNeighbor(int index)
    {
        Vector2Int position = GetNeighborPosition(index);
        return Managers.BoardManager.Hexagons[position.x, position.y];
    }

    public static Vector3 ToWorldPosition(int x, int y)
    {
        float xF = x * Constants.HorizontalDistanceBetweenHexagons;
        float zF = y * Constants.VerticalDistanceBetweenHexagons + (x % 2 == 1 ? Constants.HEXAGON_r : 0);
        return new Vector3(xF, 0f, zF);
    }

    public static Vector3 ToWorldPosition(Vector2Int position)
    {
        return ToWorldPosition(position.x, position.y);
    }
}

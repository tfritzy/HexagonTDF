using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour, Interactable
{
    public HexagonType Type;
    public Vector2Int GridPosition;

    private GameObject model;

    void Start()
    {
        SetModel();
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
}

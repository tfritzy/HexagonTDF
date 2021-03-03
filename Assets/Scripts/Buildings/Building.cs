using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : MonoBehaviour
{
    public Sprite Icon { get => Prefabs.BuildingIcons[Type]; }
    public abstract BuildingType Type { get; }
    public Vector2Int Position;

    void Start()
    {
        Setup();
    }

    protected virtual void Setup() { }

    void Update()
    {
        UpdateLoop();
    }
    protected virtual void UpdateLoop() { }
}

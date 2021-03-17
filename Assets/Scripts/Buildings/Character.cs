using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public abstract Alliances Alliance { get; }
    public abstract Alliances Enemies { get; }

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public bool IsInBuildMode { get; private set; }

    private Hexagon highlightedHexagon;

    void Start()
    {
        this.IsInBuildMode = false;
    }

    void Update()
    {
        if (IsInBuildMode)
        {
            HighlightHexagon();
        }
    }

    public void ToggleBuildMode()
    {
        IsInBuildMode = !IsInBuildMode;

        if (IsInBuildMode == false)
        {
            highlightedHexagon?.SetMaterial(Constants.Materials.Normal);
            highlightedHexagon = null;
        }
    }

    private void HighlightHexagon()
    {
        Hexagon hexagon = Helpers.FindHexByRaycast();
        if (hexagon != null)
        {
            if (hexagon != highlightedHexagon)
            {
                highlightedHexagon?.SetMaterial(Constants.Materials.Normal);
                hexagon.SetMaterial(Constants.Materials.Greyscale);
                highlightedHexagon = hexagon;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public bool IsInBuildMode { get; private set; }
    public Building SelectedBuilding
    {
        get { return selectedBuilding; }
        set
        {
            selectedBuilding = value;
            Managers.CameraControl.IsFrozen = value == null ? false : true;
            UnHighlightHexagon();
        }
    }

    private Building selectedBuilding;
    private Hexagon highlightedHexagon;
    private GameObject buildingInst;

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
            SelectedBuilding = null;
            UnHighlightHexagon();
            Managers.BuildButton.gameObject.SetActive(true);
        }
        else
        {
            Managers.BuildButton.gameObject.SetActive(false);
        }
    }

    private void UnHighlightHexagon()
    {
        Destroy(buildingInst);
        highlightedHexagon?.SetMaterial(Constants.Materials.Normal);
        highlightedHexagon = null;
    }

    private void HighlightHexagon()
    {
        if (SelectedBuilding == null)
        {
            return;
        }

        Hexagon hexagon = Helpers.FindHexByRaycast();
        if (hexagon != null)
        {
            if (hexagon != highlightedHexagon)
            {
                highlightedHexagon?.SetMaterial(Constants.Materials.Normal);
                hexagon.SetMaterial(Constants.Materials.Greyscale);
                highlightedHexagon = hexagon;
                Destroy(buildingInst);
            }
        }

        if (buildingInst == null)
        {
            buildingInst = Instantiate(SelectedBuilding.gameObject);
            buildingInst.SetMaterialsRecursively(Constants.Materials.BlueSeethrough);
        }

        buildingInst.transform.position = hexagon.transform.position;
    }
}

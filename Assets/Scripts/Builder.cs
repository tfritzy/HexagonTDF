using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public bool IsInBuildMode { get; private set; }
    public Building SelectedBuilding
    {
        get { return selectedBuilding; }
        private set
        {
            selectedBuilding = value;
            Managers.CameraControl.IsFrozen = value == null ? false : true;
            UnHighlightHexagon();
            ExitConfirmBuildMode();
            if (selectedBuilding != null)
            {
                selectBuildingTime = Time.time;
            }
        }
    }

    private float selectBuildingTime;
    private Building selectedBuilding;
    private Hexagon highlightedHexagon;
    private ButtonFunctions responsibleButton;
    private GameObject buildingInst;
    private bool isInConfirmBuild;
    private GameObject acceptButton;
    private GameObject denyButton;

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
            ExitConfirmBuildMode();
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
        if (SelectedBuilding == null || isInConfirmBuild)
        {
            return;
        }

        HighlightHexagon(Helpers.FindHexByRaycast());

        if (Input.GetMouseButtonUp(0) && Time.time - selectBuildingTime > .25f && highlightedHexagon.IsBuildable)
        {
            isInConfirmBuild = true;
            InstantiateAcceptAndDenyButtons();
        }
    }

    private void HighlightHexagon(Hexagon newPotentialHexagon)
    {
        if (newPotentialHexagon == null)
        {
            return;
        }

        if (newPotentialHexagon == highlightedHexagon)
        {
            return;
        }

        highlightedHexagon = newPotentialHexagon;

        Destroy(buildingInst);
        buildingInst = Instantiate(SelectedBuilding.gameObject);
        buildingInst.transform.position = highlightedHexagon.transform.position;

        if (highlightedHexagon.IsBuildable)
        {
            buildingInst.SetMaterialsRecursively(Constants.Materials.BlueSeethrough);
        }
        else
        {
            buildingInst.SetMaterialsRecursively(Constants.Materials.RedSeethrough);
        }
    }

    private void CreateHighlightBuilding()
    {
        Destroy(buildingInst);
        buildingInst = Instantiate(SelectedBuilding.gameObject);
        Destroy(buildingInst.GetComponent<Character>());
    }

    private void InstantiateAcceptAndDenyButtons()
    {
        float distanceBetweenButtons = 150;
        acceptButton = Instantiate(Prefabs.UIElements[UIElementType.Accept], Managers.Canvas);
        acceptButton.transform.position = Managers.Camera.WorldToScreenPoint(highlightedHexagon.transform.position) + new Vector3(150, distanceBetweenButtons / 2);

        denyButton = Instantiate(Prefabs.UIElements[UIElementType.Deny], Managers.Canvas);
        denyButton.transform.position = Managers.Camera.WorldToScreenPoint(highlightedHexagon.transform.position) + new Vector3(150, -distanceBetweenButtons / 2);
    }

    private void ExitConfirmBuildMode()
    {
        selectBuildingTime = Time.time;
        isInConfirmBuild = false;
        Destroy(acceptButton);
        Destroy(denyButton);
    }

    public void AcceptConstructBuilding()
    {
        Instantiate(selectedBuilding, highlightedHexagon.transform.position, new Quaternion());
        UnHighlightHexagon();
        ExitConfirmBuildMode();
        this.SelectedBuilding = null;
        responsibleButton.RevertIcon();
    }

    public void SetSelectedBuilding(ButtonFunctions responsibleButton, Building building)
    {
        this.SelectedBuilding = building;
        this.responsibleButton = responsibleButton;
    }

    public void DenyConstructBuilding()
    {
        ExitConfirmBuildMode();
    }
}

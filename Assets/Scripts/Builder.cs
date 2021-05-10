using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Builder : MonoBehaviour
{
    public bool IsInBuildMode { get; private set; }
    public Building SelectedBuilding
    {
        get { return selectedBuilding; }
        private set
        {
            selectedBuilding = value;
            SetCostPanels();
            UnHighlightHexagon();
            ExitConfirmBuildMode();
            if (selectedBuilding != null)
            {
                buildTargetLines.SetActive(true);
            }
            else
            {
                buildTargetLines.SetActive(false);
            }
        }
    }

    private Building selectedBuilding;
    private Hexagon highlightedHexagon;
    private ButtonFunctions responsibleButton;
    private GameObject buildingInst;
    private bool isInConfirmBuild;
    private GameObject confirmButtons;
    private GameObject buildTargetLines;
    private Dictionary<ResourceType, Text> CostPanels;

    void Start()
    {
        this.IsInBuildMode = false;
        buildTargetLines = Managers.Canvas.Find("BuildTargetLines").gameObject;
        buildTargetLines.SetActive(false);
        CostPanels = new Dictionary<ResourceType, Text>();
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            CostPanels[resourceType] = Managers.ResourceStore.transform.Find(resourceType.ToString()).Find("Cost").Find("Text").GetComponent<Text>();
        }
        SetCostPanels();
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
            buildTargetLines.SetActive(false);
            ExitConfirmBuildMode();
        }
    }

    private void UnHighlightHexagon()
    {
        Destroy(buildingInst);
        buildingInst = null;
        highlightedHexagon?.SetMaterial(Constants.Materials.Normal);
        highlightedHexagon = null;
        this.GetComponent<LineRenderer>().enabled = false;
    }

    private void HighlightHexagon()
    {
        if (SelectedBuilding == null)
        {
            return;
        }

        HighlightHexagon(Helpers.FindHexByRaycast(Constants.CenterScreen));

        if (Managers.Map.IsBuildable(highlightedHexagon.GridPosition) && (confirmButtons == null))
        {
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

        CreateHighlightBuildingIfNeeded();
    }

    private void CreateHighlightBuildingIfNeeded()
    {
        if (buildingInst == null)
        {
            buildingInst = Instantiate(SelectedBuilding.gameObject);
            Destroy(buildingInst.GetComponent<Building>());
        }

        buildingInst.transform.position = highlightedHexagon.transform.position;
        selectedBuilding.Position = highlightedHexagon.GridPosition;

        if (Managers.Map.IsBuildable(highlightedHexagon.GridPosition))
        {
            buildingInst.SetMaterialsRecursively(Constants.Materials.BlueSeethrough);

            Dictionary<Vector2Int, BuildingType> buildings = Managers.Map.GetBuildingTypeMap();
            buildings[selectedBuilding.Position] = selectedBuilding.Type;
            List<Vector2Int> newPath = Helpers.FindPath(Managers.Map.Hexagons, buildings, Managers.Map.Portals[0].Position, Managers.Map.Source.Position);
            SetupLineRenderer(newPath);
        }
        else
        {
            buildingInst.SetMaterialsRecursively(Constants.Materials.RedSeethrough);
        }

        if (selectedBuilding is AttackTower)
        {
            ((AttackTower)selectedBuilding).CreateRangeCircle(buildingInst.transform);
        }
    }

    private void InstantiateAcceptAndDenyButtons()
    {
        confirmButtons = Instantiate(Prefabs.UIElements[UIElementType.AcceptAndDeny], Managers.Canvas);
    }

    private void ExitConfirmBuildMode()
    {
        isInConfirmBuild = false;
        Destroy(confirmButtons);
    }

    public void AcceptConstructBuilding()
    {
        Vector2Int pos = highlightedHexagon.GridPosition;
        if (selectedBuilding.BuildCost.CanFulfill() && Managers.Map.IsBuildable(highlightedHexagon.GridPosition))
        {
            selectedBuilding.BuildCost.Deduct();
            Building building = Instantiate(selectedBuilding, highlightedHexagon.transform.position, new Quaternion()).GetComponent<Building>();
            building.Initialize(highlightedHexagon.GridPosition);
        }
        else
        {
            Debug.Log("Not enough resources");
        }

        UnHighlightHexagon();
    }

    public void SetSelectedBuilding(ButtonFunctions responsibleButton, Building building)
    {
        this.SelectedBuilding = building;
        this.responsibleButton = responsibleButton;
    }

    public void DenyConstructBuilding()
    {
        this.SelectedBuilding = null;
        ExitConfirmBuildMode();
        responsibleButton = null;
    }

    private void SetupLineRenderer(List<Vector2Int> path)
    {
        LineRenderer lr = this.GetComponent<LineRenderer>();
        lr.enabled = true;
        lr.positionCount = path.Count + 1;
        lr.SetPosition(0, this.transform.position);
        for (int i = 1; i < lr.positionCount; i++)
        {
            Vector2Int pos = path[i - 1];
            lr.SetPosition(i, Managers.Map.Hexagons[pos.x, pos.y].transform.position + Vector3.up * .01f);
        }
    }

    private void SetCostPanels()
    {
        foreach (Text panel in CostPanels.Values)
        {
            panel.transform.parent.gameObject.SetActive(false);
        }

        if (selectedBuilding != null)
        {
            foreach (ResourceType resource in selectedBuilding.BuildCost.Costs.Keys)
            {
                CostPanels[resource].transform.parent.gameObject.SetActive(true);
                CostPanels[resource].text = $"-{selectedBuilding.BuildCost.Costs[resource].ToString()}";
            }
        }
    }
}
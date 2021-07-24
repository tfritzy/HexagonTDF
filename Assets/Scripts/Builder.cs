using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Builder : MonoBehaviour
{
    public Building SelectedBuilding
    {
        get { return selectedBuilding; }
        private set
        {
            selectedBuilding = value;
            SetCostPanels();
        }
    }

    private Building selectedBuilding;
    private HexagonMono highlightedHexagon;
    private ButtonFunctions responsibleButton;
    private GameObject buildingInst;
    private bool isInConfirmBuild;
    private GameObject confirmButtons;
    private Dictionary<ResourceType, Text> CostPanels;
    private GameObject menu;
    private float menuCloseTime;

    void Start()
    {
        CostPanels = new Dictionary<ResourceType, Text>();
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            CostPanels[resourceType] = Managers.ResourceStore.transform.Find(resourceType.ToString()).Find("Cost").Find("Text").GetComponent<Text>();
        }
        SetCostPanels();

        menu = Prefabs.UIElements[UIElementType.AttackTowerBuildMenu];
        menu.SetActive(false);
    }

    private void UnHighlightHexagon()
    {
        Destroy(buildingInst);
        highlightedHexagon?.ResetMaterial();
        buildingInst = null;
        highlightedHexagon = null;
    }

    public bool InformHexWasClicked(HexagonMono hex)
    {
        HexagonMono clickedHex = Helpers.FindHexByRaycast(Input.mousePosition);
        SetBuildTargetHex(clickedHex);
        return clickedHex != null;
    }

    private void SetBuildTargetHex(HexagonMono newPotentialHexagon)
    {
        if (Time.time < menuCloseTime + .1f)
        {
            return;
        }

        if (newPotentialHexagon == null)
        {
            return;
        }

        if (newPotentialHexagon == highlightedHexagon)
        {
            return;
        }

        this.SelectedBuilding = null;
        UnHighlightHexagon();

        menu.SetActive(true);
        highlightedHexagon = newPotentialHexagon;
        highlightedHexagon.SetMaterial(Constants.Materials.Gold);
    }

    private void CreateHighlightBuilding(bool isBuildable)
    {
        if (buildingInst == null)
        {
            buildingInst = Instantiate(SelectedBuilding.gameObject);
            Destroy(buildingInst.GetComponent<Building>());
        }

        buildingInst.transform.position = highlightedHexagon.transform.position;
        selectedBuilding.GridPosition = highlightedHexagon.GridPosition;

        if (isBuildable)
        {
            buildingInst.SetMaterialsRecursively(Constants.Materials.BlueSeethrough);
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

    private void ExitConfirmBuildMode()
    {
        isInConfirmBuild = false;
    }

    public void AcceptConstructBuilding()
    {
        Vector2Int pos = highlightedHexagon.GridPosition;
        if (selectedBuilding.BuildCost.CanFulfill() && Managers.Board.IsBuildable(highlightedHexagon.GridPosition))
        {
            selectedBuilding.BuildCost.Deduct();
            Building building = Instantiate(selectedBuilding, highlightedHexagon.transform.position, new Quaternion()).GetComponent<Building>();
            building.Initialize(highlightedHexagon.GridPosition);
        }
        else
        {
            Debug.Log("Not enough resources, or invalid position");
        }

        ExitBuildDialog();
    }

    public void SetSelectedBuilding(ButtonFunctions responsibleButton, Building building)
    {
        if (this.SelectedBuilding != building)
        {
            Destroy(buildingInst);
            buildingInst = null;
        }

        this.SelectedBuilding = building;
        this.responsibleButton = responsibleButton;

        bool isBuildable = Managers.Board.IsBuildable(highlightedHexagon.GridPosition);

        if (isBuildable)
        {
            // TODO: open building stats page.
        }

        CreateHighlightBuilding(isBuildable);
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

    public void ExitBuildDialog()
    {
        UnHighlightHexagon();
        this.SelectedBuilding = null;
        ExitConfirmBuildMode();
        this.menu?.SetActive(false);
        menuCloseTime = Time.time;
    }
}